using Core;

var agent = new Agent
{
    Controller = new ManualController(),
};

var game = new GameClient(agent);