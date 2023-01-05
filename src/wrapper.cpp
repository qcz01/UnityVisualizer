#include "pybind11/pybind11.h"
#include <pybind11/stl.h>
#include "Simulator.hpp"
#include "utils.hpp"
namespace py = pybind11;


PYBIND11_MODULE(MAPFSim, m ){
    m.doc()="MAPF Simulator";
    py::class_<Simulator>(m,"Simulator")
        .def(py::init())
        .def(py::init<std::string>())
        //.def(py::init<int,int,int,int,std::vector<std::pair<int,int>>())
        .def("step",&Simulator::step,"step ")
        .def("getNumAgents",&Simulator::getNumAgents,"get number of agents")
        .def("addObstacle",&Simulator::addObstacle,"add obstacle")
        .def("addAgent",&Simulator::addAgent,"add agent")
        .def("getPositions",&Simulator::getPositions,"get Positions")
        .def("getPosition",&Simulator::getPosition,"get Position")
        .def("getOrientation",&Simulator::getOrientation,"get Orientation")
        .def("setRotationCost",&Simulator::setRotationCost,"set rotation cost")
        .def("getNumObstacles",&Simulator::getNumObstacles,"num obstacles")
        .def("setEnvSize",&Simulator::setEnvSize,"set map size")
        .def("addGoalToAgent",&Simulator::addGoalToAgent,"add goal to agent")
        .def("getCurrentGoalOfAgent",&Simulator::getCurrentGoalOfAgent,"get current goal of agent")
        .def("getNextCorrectedAction",&Simulator::getNextCorrectedAction,"get next corrected action")
        .def("checkConfigFeasible",&Simulator::checkConfigFeasible,"check feasible");

    py::enum_<ACTION>(m,"ACTION")
        .value("WAIT",ACTION::WAIT)
        .value("UP",ACTION::UP)
        .value("LEFT",ACTION::LEFT)
        .value("DOWN",ACTION::DOWN)
        .value("RIGHT",ACTION::RIGHT)
        .value("FORWARD",ACTION::FORWARD)
        .value("CLOCKWISE",ACTION::CLOCKWISE)
        .value("COUNTERCLOCK",ACTION::COUNTERCLOCK)
        .export_values();

    m.def("read_map",&read_map);
    m.def("read_instance",&read_instance);
    m.def("read_paths",&read_paths);
    m.def("pos_to_actions",&pos_to_actions);
    m.def("read_actions",&read_actions);
}