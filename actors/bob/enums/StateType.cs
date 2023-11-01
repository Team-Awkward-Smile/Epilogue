using Godot;
using System;

namespace Epilogue.actors.hestmor.enums;
/// <summary>
///		Represents different information about States
/// </summary>
public enum StateType
{
	/// <summary>
	///		A jump that goes directly up
	/// </summary>
	VerticalJump,

	/// <summary>
	///		A jump that goes up and forward
	/// </summary>
	LowJump,

	/// <summary>
	///		A long jump forward
	/// </summary>
	LongJump,

	/// <summary>
	///		A swipe with claws
	/// </summary>
	SwipeAttack,

	/// <summary>
	///		An uppercut punch
	/// </summary>
	UppercutPunch,

	/// <summary>
	///		An attack performed during a slide
	/// </summary>
	SlideAttack,

	/// <summary>
	///		A small slide forward
	/// </summary>
	FrontRoll,

	/// <summary>
	///		A medium slide forward
	/// </summary>
	KneeSlide,

	/// <summary>
	///		A long slide forward
	/// </summary>
	LongSlide
}
