import numpy as np
import MAPFSim
import argparse
import zmq
import time
import uuid
import json
from subprocess import STDOUT, check_output
import threading

#unity exe directory
unity_exe_dir="./MAPF-2D.exe"


def test_read_actions():
    actions=MAPFSim.read_actions("./demo32x32/demo_rotate.txt")
    print(len(actions))
    

def test1():
    sim=MAPFSim.Simulator()
    xmax=32
    ymax=32
    obstacles=[(3*i+1,3*j+1) for i in range(0,xmax,3) for j in range(0,ymax,3)]
    starts=[(0,0)]
  
    actions=[MAPFSim.ACTION.RIGHT]
    for start in starts:
        sim.addAgent(start[0],start[1],-1)
    for obstacle in obstacles:
        sim.addObstacle(obstacle[0],obstacle[1])
    sim.setEnvSize(0,xmax,0,ymax)
    vi=sim.getPosition(0)
    print("map size (xmin,xmax),(ymin,ymax)",())
    print("num agents=",sim.getNumAgents())
    print("current position of agent",0,":",vi)
    print("num obstacles=",sim.getNumObstacles())
    sim.step(actions)
    print("current position of agent",0,":",vi)
    # print("step....")
    actions=[MAPFSim.ACTION.UP]
    # print(int(actions[0]))
    sim.step(actions)
    vi=sim.getPosition(0)
    print("current position of agent",0,":",vi)

    

def zmq_send_actios(actions):
    context = zmq.Context()
    # socket = context.socket(zmq.REP)
    socket = context.socket(zmq.PUB)
    print("socket binded","tcp://*:5556")
    socket.bind("tcp://*:5556")
    # socket.recv()
    print("??????")
    test_dict=dict()
    for i,action in enumerate(actions):
        test_dict[i]=int(action)
    while True:
        test_dict=dict()
        for i,action in enumerate(actions):
            test_dict[i]=np.random.randint(10)
        socket.send_string(str(json.dumps(test_dict)))
        
        print(str(json.dumps(test_dict)))
        time.sleep(1)
    print("sent string")
    
    

def test_zmq():
    actions=[MAPFSim.ACTION.WAIT,MAPFSim.ACTION.WAIT,MAPFSim.ACTION.WAIT]
    zmq_send_actios(actions)


def test_zmq_with_unity(paths_file):
    
    # paths_file="C:/Users/GREATEN/MAPF-2D/Assets/Resources/PathFile/den312_demo.txt"
    paths=MAPFSim.read_paths(paths_file)
    # print(len(paths))
    context = zmq.Context()
    # for path in paths:
    #     print(path)
    actions=[]
    for i,path in enumerate(paths):
        action=MAPFSim.pos_to_actions(path)
        actions.append(action)
        # print(i,action)
    socket = context.socket(zmq.PUB)
    print("socket binded","tcp://*:5556")
    socket.bind("tcp://*:5556")
    t=0
    time.sleep(10)
    while t<len(actions[0]):
        test_dict=dict()
        for i,action in enumerate(actions):
            test_dict[i]=int(action[t])
        socket.send_string(str(json.dumps(test_dict)))
        t+=1
        # print(str(json.dumps(test_dict)))
        time.sleep(0.2)


def zmq_send_actions_with_unity(actions_file):
    
    # paths_file="C:/Users/GREATEN/MAPF-2D/Assets/Resources/PathFile/den312_demo.txt"
    actions=MAPFSim.read_actions(actions_file)
    makespan=max([len(p) for p in actions])
    # print(len(paths))
    context = zmq.Context()
    # for path in paths:
    #     print(path)

        # print(i,action)
    socket = context.socket(zmq.PUB)
    print("socket binded","tcp://*:5556")
    socket.bind("tcp://*:5556")
    t=0
    time.sleep(10)
    while t<makespan:
        test_dict=dict()
        for i,action in enumerate(actions):
            if t<len(action):
                test_dict[i]=int(action[t])
            else:
                test_dict[i]=int(MAPFSim.ACTION.WAIT)
        socket.send_string(str(json.dumps(test_dict)))
        t+=1
        print(t)
        # print(str(json.dumps(test_dict)))
        time.sleep(0.2)


def open_gui(args):
    cmd = [unity_exe_dir,"--map",args.map,"--instance",args.instance,"--speed",str(args.speed),"--labels",str(args.labels),"--rotation_cost",str(args.rotation_cost)]
    print(cmd)
    check_output(cmd, stderr=STDOUT, timeout=100).decode("utf-8")

def json_example():
    data_dict=dict()
    data_dict["map"]="./Assets/Maps/random-32-32-10.map"
    data_dict["instance"]="./Assets/Instances/random-32-32-10/random-32-32-10_agents100.scen"
    data_dict["paths"]="./Assets/PathFile/random-32-32-10_demo.txt"
    data_dict["actions"]=""
    data_dict["gui"]=True
    data_dict["speed"]=1
    data_dict["labels"]=False
    with open("./example.json",'w') as f:
        json.dump(data_dict,f, indent=2)



def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--map', type=str, default=None, help="output video file (or leave empty to show on screen)")
    parser.add_argument("--instance", type=str, default=None, help="instance file")
    parser.add_argument("--paths", type=str, default=None, help="paths file")
    parser.add_argument("--actions", type=str, default=None, help="actions file")
    parser.add_argument("--speed", type=int, default=1, help="visualization speed")
    parser.add_argument("--gui",type=bool,default=False,help="run gui")
    parser.add_argument("--json",type=str,default=None,help="all configs in json")
    parser.add_argument("--labels",type=bool,default=False,help="show labels")
    parser.add_argument("--rotation_cost",type=int,default=0,help="rotation cost")
    
    args = parser.parse_args()
    if args.json is not None:
        with open(args.json,"r") as f:
            cmd_dict=json.load(f)
            args.map=cmd_dict['map']
            args.instance=cmd_dict['instance']
            args.paths=cmd_dict['paths']
            args.gui=cmd_dict['gui']
            args.labels=cmd_dict['labels']
            args.speed=cmd_dict['speed']
            args.actions=cmd_dict['actions']
            try:
                args.rotation_cost=cmd_dict["rotation_cost"]
            except:
                args.rotation_cost=0

    if args.map is not None:
        if args.instance is not None:
            if args.gui !=False:
                t = threading.Thread(target=open_gui,args=(args,))
                t.start()
                time.sleep(1)
                if args.paths is not None and len(args.paths)!=0:
                    print(args.paths)
                    test_zmq_with_unity(args.paths)
                if args.actions is not None and len(args.actions)!=0:
                    print(args.actions)
                    zmq_send_actions_with_unity(args.actions)
                t.join()

    else:
        paths_file="C:/Users/GREATEN/MAPF-2D/Assets/Resources/PathFile/den312_demo.txt"
        test_zmq_with_unity(paths_file)
 
            




    


if __name__=="__main__":
    # test1()
    
    # test_zmq_with_unity()
    main()
    # test_read_actions()
    # json_example()