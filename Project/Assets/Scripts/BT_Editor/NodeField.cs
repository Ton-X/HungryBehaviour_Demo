using System;

[AttributeUsage(AttributeTargets.Field)]
public class NodeField : Attribute {

	public bool IsNodeField { 
		get{ return isNodeField;}
	}

	private bool isNodeField = true;
}

