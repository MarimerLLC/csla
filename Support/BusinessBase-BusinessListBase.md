# Inheritance Hierarchy: BusinessBase vs BusinessListBase

This diagram compares the inheritance hierarchies of `BusinessBase` and `BusinessListBase`, ignoring interfaces.
```mermaid
%% Legend:
%% [csla] = in your codebase
%% [bcl] = .NET BCL
%% [nuget] = NuGet package
%% <|.. = implements interface

classDiagram
    object <|-- MobileObject
    class object {<<bcl>>}
```mermaid
%% Legend:
%% [csla] = in your codebase
%% [bcl] = .NET BCL
%% [nuget] = NuGet package
%% <|.. = implements interface

classDiagram
    object <|-- MobileObject
    MobileObject <|-- BindableBase
    BindableBase <|-- UndoableBase
    UndoableBase <|-- BusinessBase
    BusinessBase <|-- BusinessBaseOfT
    object <|-- CollectionOfT
    CollectionOfT <|-- ObservableCollectionOfT
    ObservableCollectionOfT <|-- MobileObservableCollectionOfT
    MobileObservableCollectionOfT <|-- ObservableBindingListOfT
    ObservableBindingListOfT <|-- BusinessListBaseOfT_C

    %% Interfaces (partial, main ones for clarity)
    BindableBase <|.. INotifyPropertyChanged
    BindableBase <|.. INotifyPropertyChanging
    UndoableBase <|.. IUndoableObject
    UndoableBase <|.. IUseApplicationContext
    BusinessBase <|.. ISavable
    BusinessBase <|.. ISavableOfT
    BusinessBase <|.. IBusinessBase
    BusinessBaseOfT <|.. ISavableOfT
    MobileObservableCollectionOfT <|.. IMobileList
    ObservableBindingListOfT <|.. IObservableBindingList
    ObservableBindingListOfT <|.. INotifyBusy
    ObservableBindingListOfT <|.. INotifyChildChanged
    ObservableBindingListOfT <|.. ISerializationNotification

    %% Declare all nodes for styling
    class object
    class MobileObject
    class BindableBase
    class UndoableBase
    class BusinessBase
    class BusinessBaseOfT
    class CollectionOfT
    class ObservableCollectionOfT
    class MobileObservableCollectionOfT
    class ObservableBindingListOfT
    class BusinessListBaseOfT_C
    class INotifyPropertyChanged
    class INotifyPropertyChanging
    class IUndoableObject
    class IUseApplicationContext
    class ISavable
    class ISavableOfT
    class IBusinessBase
    class IMobileList
    class IObservableBindingList
    class INotifyBusy
    class INotifyChildChanged
    class ISerializationNotification

classDef interface fill:#e6f0ff,stroke:#1a4d80,stroke-width:2px
classDef csla fill:#f5f5f5,stroke:#888,stroke-width:2px
classDef bcl fill:#f0f0ff,stroke:#888,stroke-width:2px

class INotifyPropertyChanged interface
class INotifyPropertyChanging interface
class IUndoableObject interface
class IUseApplicationContext interface
class ISavable interface
class ISavableOfT interface
class IBusinessBase interface
class IMobileList interface
class IObservableBindingList interface
class INotifyBusy interface
class INotifyChildChanged interface
class ISerializationNotification interface
class MobileObject csla
class BindableBase csla
class UndoableBase csla
class BusinessBase csla
class BusinessBaseOfT csla
class MobileObservableCollectionOfT csla
class ObservableBindingListOfT csla
class BusinessListBaseOfT_C csla
class object bcl
class CollectionOfT bcl
class ObservableCollectionOfT bcl
class UndoableBase csla;
