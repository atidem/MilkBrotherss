# -*- coding: utf-8 -*-
"""
@author: atidem
"""
from keras.models import Sequential
from keras.layers.core import Dense,Activation,Flatten 
from keras.layers.convolutional import Conv2D
#from keras.optimizers import RMSprop
from collections import deque 
from skimage import color,transform,exposure
import random
import numpy as np 
import warnings 
warnings.filterwarnings("ignore")

#%%
## parameters 
## determined with hardware limits

actionSize = 4
imgHeight = 84 
imgWidth = 84
imgHistory = 4

obsPeriod = 16 
gamma = 0.95
batchSize = 16 

expReplayCapacity = 10000
#%%
class Agent:
    def __init__(self):
        ## Decision Maker
        self.model = self.createModel()
        ## Memory of Agent
        self.experReplay = deque()
        ## used when 200 game pre-training
        self.steps = 0 
        ## changed manuel ## 200 game 1 , 300 game 0.75 ,500 game 0.5 ,750 game 0.2,1000 game 0.1 
        self.epsilon = 1  
    
    def createModel(self):
        ## cnn model created
        model = Sequential()
        model.add(Conv2D(64,kernel_size=(4,4),strides=2,input_shape=(imgHeight,imgWidth,imgHistory),padding="same"))
        model.add(Activation("relu"))
        model.add(Conv2D(64,kernel_size=(4,4),strides=2,padding="same"))
        model.add(Activation("relu"))
        model.add(Conv2D(64,kernel_size=(3,3),strides=1,padding="same"))
        model.add(Activation("relu"))
        model.add(Flatten())
        model.add(Dense(512))
        model.add(Activation("relu"))
        model.add(Dense(units=actionSize,activation="linear"))
        #rms = RMSprop(learning_rate=0.01,rho=0.9)
        model.compile(loss="mse",optimizer='adam')
        return model
        
    def chooseAct(self,state):
        ## explore and exploit
        if random.random()<self.epsilon:
            return random.randint(0,actionSize-1)
        else:
            qValues = self.model.predict(state)
            best = np.argmax(qValues)
            return best

    def captureSample(self,data):
        ## fill experience replay memory
        self.experReplay.append(data)
        if len(self.experReplay) > expReplayCapacity:
            self.experReplay.popleft()

        self.steps += 1
        
## going down randomness (adaptive epsilon for training)
## used when 200 game pre-training
#        if self.steps > obsPeriod:
#            if self.steps > 7000:
#                self.epsilon = 0.05
#            elif self.steps > 5000:
#                self.epsilon = 0.1
#            elif self.steps > 3000:
#                self.epsilon = 0.2
#            elif self.steps > 1400:
#                self.epsilon = 0.5
#            elif self.steps > 700:
#                self.epsilon = 0.75          

    def processImage(self,img):
        ## 84*84 and grayscale
        img = np.asanyarray(img)
        GreyImage = color.rgb2gray(img)
        ReducedImage = transform.resize(GreyImage,(imgHeight,imgWidth))
        ReducedImage = exposure.rescale_intensity(ReducedImage, out_range = (0,255))
        ReducedImage = ReducedImage / 128
        return ReducedImage

    def process(self):
        ## training time
        if self.steps > obsPeriod:
            minibatch = random.sample(self.experReplay,batchSize)
            batchlen = batchSize
            
            xValues = np.zeros((batchSize,imgHeight,imgWidth,imgHistory))
            yValues = np.zeros((xValues.shape[0],actionSize))
            Q_sa = 0
            
            for i in range(batchlen):
                stateT = minibatch[i][0]
                actionT = minibatch[i][1]
                rewardT = minibatch[i][2]
                stateT1 = minibatch[i][3]
                
                xValues[i:i+1] = stateT
                yValues[i] = self.model.predict(stateT)
                Q_sa = self.model.predict(stateT1)
                
                if stateT1 is None:
                    yValues[i,actionT] = rewardT
                else:
                    yValues[i,actionT] = rewardT + gamma*np.max(Q_sa)
                
                self.model.fit(xValues,yValues,batch_size=batchSize,epochs=1,verbose=0)
