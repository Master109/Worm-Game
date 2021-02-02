using System;

public struct LimitWhileLoopIterationCount
{
	public Action whileLoopContent;

	public LimitWhileLoopIterationCount (Action whileLoopContent)
	{
		this.whileLoopContent = whileLoopContent;
	}
}