using Csla;
using System;

// These should cause an issue.
public class ClassIsStereotypeAndIsNotSerializable
    : BusinessBase<ClassIsStereotypeAndIsSerializable>
{ }

// These should not.
public class ClassIsNotStereotype { }

[Serializable]
public class ClassIsStereotypeAndIsSerializable
    : BusinessBase<ClassIsStereotypeAndIsSerializable>
{ }