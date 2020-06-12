# -*- coding: utf-8 -*-
"""
@author: atidem
"""
#socket to tcp://*:5557
import agent
import time
import zmq
import numpy as np 
from keras.models import load_model
import keras.backend as K
import warnings
warnings.filterwarnings("ignore")
from PIL import Image 
import winsound

#%% constant variable
playTime = 51
imgH = 84  
imgW = 84
imgHist = 4

#%% connection and training time
context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5557")
move = ["r","l","u","d"]

itr = 0
server = 0 
spurt = 0
inputs = []

## agent included
agentP = agent.Agent()
agentP.epsilon=0.1

## load saved model and weights
agentP.model = load_model("milkVer3.h5")
print("! milkVer3 !")

agentP.model.summary()

while True:
    if(server==0):
        print(" Agent Activated ..")
        
    ## if there is not first screen img  
    message = socket.recv()
    inputs = str(message).strip("b'").split()
    firstScreen = agentP.processImage(Image.open("main.png"))
    state = np.stack((firstScreen,firstScreen,firstScreen,firstScreen),axis = 2)
    state = state.reshape(1, state.shape[0],state.shape[1],state.shape[2])
    
    server += 1
    ###if there is a message from Client , do something cool
    if(len(inputs)>0):
        while (itr<playTime):
            if(len(inputs)>0):
                if(spurt==0):
                    print("! Playing.. !")
                
                #choose act
                bestMove = agentP.chooseAct(state)
                
                #send move / apply
                socket.send_string(move[bestMove]) 
                
                # Handle img process error 
                try:
                    img = agentP.processImage(Image.open(inputs[0]+".png"))
                except FileNotFoundError:
                    print("! {}.spurt there is no img, game state :{} ".format(inputs[0],inputs[2]))
                    img = agentP.processImage(Image.open("black.png"))
                except PermissionError:
                    try:
                        print("! {}.img permission denied, game state :{} ".format(inputs[0],inputs[2]))
                        time.sleep(0.1)
                        img = agentP.processImage(Image.open(inputs[0]+".png"))
                    except PermissionError:
                        print("! {}.img permission denied twice !")
                except MemoryError:
                    print("! {}. Screenshot broken".format(inputs[0]))
                    winsound.Beep(2500,100)
                
                # img reshaped for mr. keras
                img = img.reshape(1,img.shape[0],img.shape[1],1)
                
                #state stack , append new screenshot
                nState = np.append(img, state[:,:,:,:3], axis = 3)
                
                #save exp.Rep.sample
                agentP.captureSample((state,bestMove,float(inputs[1].replace(",",".")),nState))
                
                #state = next state
                state = nState
                
                ##when game finished , write final score to txt and screen , and mod5 for save cnn model
                if(inputs[2]== 'False'):
                    print(str(itr) +".game , Total spurt:"+inputs[0]+" , Total Score: "+ str(inputs[1]))
                    print("--------------new game-----------------")
                    itr += 1
                spurt += 1  
                    
                message = socket.recv()
                inputs = str(message).strip("b'").split()
        socket.send_string("end")
        winsound.Beep(2500,1500)
        break
    else:
        print("there is no message from client...")
        
print("to be continue..")
