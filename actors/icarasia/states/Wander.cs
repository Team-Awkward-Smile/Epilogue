using Epilogue.nodes;
using Godot;
using System;

namespace Epilogue.actors.icarasia.states;
public partial class Wander : NpcState
{
    [Export] private float _wanderSpeed = 50f;
}
