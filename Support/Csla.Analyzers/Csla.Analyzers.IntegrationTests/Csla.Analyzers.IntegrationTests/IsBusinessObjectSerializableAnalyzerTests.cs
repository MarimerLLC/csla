using Csla;
using System;

public class ClassIsStereotypeAndIsNotSerializable
    : BusinessBase<ClassIsStereotypeAndIsSerializable>
{ }

public class ClassIsNotStereotype { }

[Serializable]
public class ClassIsStereotypeAndIsSerializable
    : BusinessBase<ClassIsStereotypeAndIsSerializable>
{ }