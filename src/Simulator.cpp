#include "Simulator.hpp"
#include "json.hpp"

using point2d=std::pair<int,int>;

/// @brief 
Simulator::Simulator(){

}
/// @brief 
/// @param filename 
Simulator::Simulator(std::string filename){
    nlohmann::json data_dict;
}

/// @brief 
/// @param xmin 
/// @param xmax 
/// @param ymin 
/// @param ymax 
/// @param obstacles 
Simulator::Simulator(int xmax,int ymax,std::vector<point2d> obstacles){
    // this->xmin=xmin;
    this->xmax=xmax;
    // this->ymin=ymin;
    this->ymax=ymax;
    // for(auto obs:obstacles){
    //     obstacles.insert(obs);
    // }

}

/// @brief 
/// @param x 
/// @param y 
/// @param direction 
/// @return 
int Simulator::addAgent(int x,int y,int direction){
    if(obstacles.find({x,y})!=obstacles.end()){
        printf("Hit obstacle: (%d,%d)\n",x,y);
        return -1;
    }
    if(pos_dict.find({x,y})==pos_dict.end()){
        config.push_back({x,y});
        directions.push_back(direction);
        pos_dict[{x,y}]=config.size()-1;
        rotation_counter.push_back(0);
        
    }
    return config.size()-1;
}


/// @brief 
/// @return 
int Simulator::getNumObstacles(){
    return obstacles.size();
}

/// @brief 
/// @param agent 
/// @return 
point2d Simulator::getPosition(int agent){
    return config[agent];
}


/// @brief 
/// @return 
int Simulator::getNumAgents(){
    return config.size();
}


/// @brief 
/// @param x 
/// @param y 
/// @return 
int Simulator::addObstacle(int x,int y){
    obstacles.insert({x,y});
    return obstacles.size();
}


/// @brief 
/// @param actions 
void Simulator::step(std::vector<ACTION> actions){
    // printf(" step   ??????\n");
    nextPos=config;
    
    Config tmp_next=getNextPosition(actions);
    
    // printf("next porb pos=(%d,%d)\n",tmp_next[0].first,tmp_next[0].second);
    std::set<point2d> occupied;
    std::set<int> moved;
    int numAgents=config.size();
    
    auto stopAgent=[&](int i){
        nextPos[i]=config[i];
        moved.insert(i);
        occupied.insert(config[i]);
    };
    auto forwardAgent=[&](int i){
        // printf("forward agent %d\n",i);
        nextPos[i]=tmp_next[i];
        // printf("forwarded agent %d\n",i);
        moved.insert(i);
        occupied.insert(nextPos[i]);
     
    };


    //check if conflict with obstacles
    //brute force
    for(int i=0;i<numAgents;i++){
        int xi=tmp_next[i].first;
        int yi=tmp_next[i].second;
        if(xi<xmin || xi>=xmax||yi<ymin ||yi>=ymax){

            stopAgent(i);
        }
        else{
            if(obstacles.find(tmp_next[i])!=obstacles.end()){
                stopAgent(i);
             
            }
        }
    }

    //stop conflicted agents
    for(int i=0;i<numAgents;i++){
        for(int j=i+1;j<numAgents;j++){
            //vertex
            if(tmp_next[i]==tmp_next[j]){
                stopAgent(i);
                stopAgent(j);
             
            }

            //edge
            if(config[i]==tmp_next[j] && config[j]==tmp_next[i]){
                stopAgent(i);
                stopAgent(j);
               
            }
        }
    }

    //move other agents

    for(int i=0;i<numAgents;i++){
        //moved
        if(moved.find(i)!=moved.end()) continue;
        //delayed by other agents
        if(occupied.find(tmp_next[i])!=occupied.end()){
            stopAgent(i);
        }
        else{  
            forwardAgent(i);
        }
    }

    config=nextPos;
    pos_dict.clear();
    //update occupation 
    for(int i=0;i<numAgents;i++) pos_dict[config[i]]=i;
    // assert(checkConfigFeasible(config));

}


/// @brief 
/// @param configx 
/// @return 
bool Simulator::checkConfigFeasible(const Config&configx){
    int num_agents=configx.size();
    std::set<point2d> occupied;
    for(int i=0;i<num_agents;i++){
        if (configx[i].first < xmin || configx[i].first >= xmax || configx[i].second < ymin || configx[i].second >= ymax) return false;
        if (obstacles.find(configx[i]) != obstacles.end()) return false;
        if(occupied.find(configx[i])==occupied.end())occupied.insert(configx[i]);
        else return false;
    }
    return true;
}


/// @brief 
/// @param agent 
/// @param action 
/// @return 
std::pair<point2d,int>Simulator::getNextState(int agent,ACTION action){
    point2d new_pos=config[agent];
    // printf("debug agent (%d,%d)\n",new_pos.first,new_pos.second);
    int new_direction=directions[agent];
    if(directions[agent]==-1){
        switch (action)
        {
            case ACTION::WAIT:
                break;
            case ACTION::LEFT:
                new_pos.first--;
                break;
            case ACTION::UP:
                new_pos.second++;
                break;
            case ACTION::DOWN:
                new_pos.second--;
                break;
            case ACTION::RIGHT:
                new_pos.first++;
                break;
            default:
                break;
        }
    }
    //consider rotation
    else{
        if(rotation_counter[agent]!=0)//still rotating
        {
            rotation_counter[agent]--;
        }else{
            switch(action){
                case ACTION:: WAIT:
                    break;
                case ACTION:: FORWARD:
                    new_pos=forwardRotateAgent(agent);
                    break;
                case ACTION::CLOCKWISE:
                    new_direction=(new_direction+1)%4;
                    break;
                case ACTION::COUNTERCLOCK:
                    new_direction=(new_direction-1)%4;
                    break;
            }
        }
    }

    return {new_pos,new_direction};
}



point2d Simulator::forwardRotateAgent(int agent){
    point2d new_pos=config[agent];
    switch (directions[agent])
    {
        case 0:
            new_pos.first++;
            break;
        case 1:
            new_pos.second--;
            break;
        case 2:
            new_pos.first--;
            break;
        case 3:
            new_pos.second++;
            break;
        default:
            break;
    }
    return new_pos;

}

/// @brief 
/// @param actions 
/// @return 
Simulator::Config Simulator::getNextPosition(std::vector<ACTION> actions){
    Config nextConfig;
    for(int i=0;i<config.size();i++){
        auto nextstate=getNextState(i,actions[i]);
        auto nextPos=nextstate.first;
    
        auto angle=nextstate.second;
        directions[i]=angle;
        nextConfig.push_back(nextPos);
    }
    return nextConfig;
}


/// @brief 
/// @return 
Simulator::Config Simulator::getPositions(){
    return config;
}

/// @brief 
/// @param agent 
/// @return 
int Simulator::getOrientation(int agent){
    return directions[agent];
}

/// @brief 
/// @param xmin 
/// @param xmax 
/// @param ymin 
/// @param ymax 
void Simulator::setEnvSize(int xmax,int ymax){
    // this->xmin=xmin;
    this->xmax=xmax;
    // this->ymin=ymin;
    this->ymax=ymax;
}


/// @brief 
/// @param rotation_cost 
void Simulator::setRotationCost(int rotation_cost){
    this->rotation_cost=rotation_cost;
}