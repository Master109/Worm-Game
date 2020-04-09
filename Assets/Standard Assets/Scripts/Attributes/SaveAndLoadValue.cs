using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SaveAndLoadValue : Attribute
{
}
