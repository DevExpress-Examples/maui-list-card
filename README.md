<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/610334985/24.1.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1153128)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->
# Expandable Cards with Lists

This example shows you how to implement a List Card - a UI element that allows you to preview several items from a lengthy list, access the complete list, and execute custom actions. 

<img width="50%" src="https://user-images.githubusercontent.com/12169834/223110944-4904bf34-da91-4685-9656-fb7e09905d42.png"/>

## Used Controls and Their Properties

**A. Header**

  * [Label](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/label?view=net-maui-7.0): [Text](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.label.text?view=net-maui-7.0)
  * [DXButton](https://docs.devexpress.com/MAUI/DevExpress.Maui.Core.DXButton): [Command](https://docs.devexpress.com/MAUI/DevExpress.Maui.Core.DXButtonBase.Command)

**B. List Items**

  * [Label](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/label?view=net-maui-7.0): [Text](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.label.text?view=net-maui-7.0)
  * [DXButton](https://docs.devexpress.com/MAUI/DevExpress.Maui.Core.DXButton): [Command](https://docs.devexpress.com/MAUI/DevExpress.Maui.Core.DXButtonBase.Command), [CommandParameter](https://docs.devexpress.com/MAUI/DevExpress.Maui.Core.DXButtonBase.CommandParameter)
  * [CollectionView](https://docs.devexpress.com/MAUI/DevExpress.Maui.CollectionView.DXCollectionView): [CollectionView.ItemsSource](https://docs.devexpress.com/MAUI/DevExpress.Maui.CollectionView.DXCollectionView.ItemsSource), [CollectionView.ItemTemplate](https://docs.devexpress.com/MAUI/DevExpress.Maui.CollectionView.DXCollectionView.ItemTemplate)

**C. Footer (optional)**

  * [DXButton](https://docs.devexpress.com/MAUI/DevExpress.Maui.Core.DXButton): [Command](https://docs.devexpress.com/MAUI/DevExpress.Maui.Core.DXButtonBase.Command)
    
## A. Header

### Anatomy
Users can click the header area to access the complete list.

<img width="50%" src="https://user-images.githubusercontent.com/12169834/223118601-0386f5c7-fd4c-4fe1-a03d-775b5e12a9e6.png"/>

### Behavior

Follow the steps below to handle header clicks and implement navigation to the screen with the complete list of items:

1. Call the [Routing.RegisterRoute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.routing.registerroute?view=net-maui-7.0#microsoft-maui-controls-routing-registerroute(system-string-system-type)) method to register the page that contains the complete list of items:

    ```csharp
    public partial class App : Application {
        public App() {
            InitializeComponent();
            Routing.RegisterRoute("completeList", typeof(CompleteListPage));
            MainPage = new AppShell();
        }
    }
    ```

2. Specify the [DXButton.Command](https://docs.devexpress.com/MAUI/DevExpress.Maui.Core.DXButtonBase.Command) property to define the header click command:

    ```xaml
    <dx:DXStackLayout Orientation="Vertical">
        <dx:DXButton Content="{Binding Title}" Command="{Binding NavigateToAllCommand}" ...>
            <dx:DXButton.Content>
                <Grid ColumnDefinitions="*,Auto" HorizontalOptions="Fill" Padding="0">
                    <Label Text="{Binding Title}" .../>
                    <Label Text="{Binding Path=Items.Count, StringFormat='All ({0})'}" .../>
                </Grid>
            </dx:DXButton.Content>
        </dx:DXButton>
    </dx:DXStackLayout>
    ```
    
    ```csharp
    namespace CollectionViewWithActionButtons.ViewModels {
        // ...
        public class Card : BindableBase {
            public Card(string title) {
                // ...
                NavigateToAllCommand = new Command(NavigateToAll);
            }
            // ...
            public ICommand NavigateToAllCommand { get; }
            
            public async void NavigateToAll() {
                var navigationParameter = new Dictionary<string, object> { { "Parent", this } };
                await Shell.Current.GoToAsync("completeList", navigationParameter);
            }
        }
    }
    ```

    The **NavigateToAll** method calls the [GoToAsync](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.shell.gotoasync?view=net-maui-7.0#microsoft-maui-controls-shell-gotoasync(microsoft-maui-controls-shellnavigationstate-system-collections-generic-idictionary((system-string-system-object)))) method to open the detail view.

3. Specify the [QueryProperty](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.querypropertyattribute?view=net-maui-7.0) attribute for the **CompleteListViewModel** class to pass the clicked header context to the command registered in the previous step:
   
   ```csharp
   namespace CollectionViewWithActionButtons.ViewModels {
       [QueryProperty(nameof(ParentCard), "Parent")]
       internal class CompleteListViewModel : INotifyPropertyChanged {
           private Card parentCard;
           public Card ParentCard {
               get { return parentCard; }
               set {
                   parentCard = value;
                   OnPropertyChanged();
               }
           }
           public event PropertyChangedEventHandler PropertyChanged;
           void OnPropertyChanged([CallerMemberName] string propertyName = null) {
               PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
           }
       }
   }
   ```

## B. List Items


This area contains a list of items. You can click on an item to view its details. Click on an **✕** button to delete the corresponding item. 


<img width="50%" src="https://user-images.githubusercontent.com/12169834/223119451-8b5fb385-590c-4707-a05d-8dbd662a23cc.png"/>

### Behavior

Follow the steps below to open the **CollectionView** item detailed information on click:

1. Specify the [DXButton.Command](https://docs.devexpress.com/MAUI/DevExpress.Maui.Core.DXButtonBase.Command) and [DXButton.CommandParameter](https://docs.devexpress.com/MAUI/DevExpress.Maui.Core.DXButtonBase.CommandParameter) properties to define the item click command. The following code sample uses the [FindAncestorBindingContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.relativebindingsourcemode?view=net-maui-7.0) binding to get the command of the parent object's **ItemClick** and **Hide** commands:
   
    ```xaml
    <dxco:DXButton Command="{Binding Source={RelativeSource Mode=FindAncestorBindingContext,
                    AncestorType={x:Type viewModels:Card}}, Path=ItemClickCommand}"
                    CommandParameter="{Binding}">
        <Grid ...>
            <Image Source="{Binding Icon}" .../>
            <Label Text="{Binding Name}" .../>
        </Grid>
    </dxco:DXButton>
    <dxco:DXButton Content="&#x2715;" Command="{Binding Source= {RelativeSource Mode=FindAncestorBindingContext,
                    AncestorType={x:Type viewModels:Card}}, Path=HideCommand}" CommandParameter="{Binding}" .../>
    ```

1. Define the **ItemClick** and **Hide** commands in the **Card** class:
   
    ```csharp
    namespace CollectionViewWithActionButtons.ViewModels {
        public class ViewModel : BindableBase {
            public ViewModel() {
                Cards = DataGenerator.CreateCards();
            }
            public ObservableCollection<Card> Cards { get; set; }
        }
        
        public class Card : BindableBase {
            public Card(string title) {
                Title = title;
                HideCommand = new Command<CardItem>(HideItem);
                ItemClickCommand = new Command<CardItem>(ItemClick);
                // ...
            }

            public string Title { get; }
            public ICommand HideCommand { get; }
            public ICommand ItemClickCommand { get; }
            // ...
            public async void ItemClick(CardItem clickedItem) {
                if (clickedItem == null) return;
                await Application.Current.MainPage.DisplayAlert("Item Click", clickedItem.Name, "OK");
            }
            // ...
        }

        public class CardItem : BindableBase {
            public string Name { get; set; }
            public string Subtitle { get; set; }
            public DateTime CreatedDate { get; set; }
            public ImageSource Icon { get; set; }
        }

        public class BindableBase : INotifyPropertyChanged {
            public event PropertyChangedEventHandler PropertyChanged;
            protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    ```

## C. Footer (Optional)

The footer area includes buttons that can initiate list-level actions (such as "Hide All"). 

<img width="50%" src="https://user-images.githubusercontent.com/12169834/223119520-25279691-b6cd-4147-b3aa-8154d0b9670e.png"/>

### Behavior

Buttons within the footer are visible only when the **Card.AllowCommonActions** property is `true`. The **PrimaryActionName** and **SecondaryActionName** properties define button names. 

The following code snippet specifies whether the **Security** card's footer buttons are visible and defines their names:

```csharp
public static class DataGenerator {
    public static ObservableCollection<Card> CreateCards() {
        ObservableCollection<Card> cards = new ObservableCollection<Card>();
        cards.Add(new Card("Security") {
            Items = new ObservableCollection<CardItem>() {
                // ...
            },
            PreviewItemsCount = 4,                
            AllowCommonActions = true,
            PrimaryActionName = "Dismiss All",
            SecondaryActionName = "Apply All"
        });
        cards.Add(new Card("Performance") {  
            Items = new ObservableCollection<CardItem>() {
                // ...
            },
            PreviewItemsCount = 3,
        });
        // ...
    }
    // ...
}
```


Follow the steps below to implement commands that process all **CollectionView** items within the card:

1. Specify the [DXButton.Command](https://docs.devexpress.com/MAUI/DevExpress.Maui.Core.DXButton.Command) property to define the click commands for both footer buttons.
   
    ```xaml
    <dx:DXStackLayout Orientation="Horizontal" HorizontalOptions="End" x:Name="commonActionsPanel" Padding="20,5,20,0">
        <dx:DXStackLayout.Triggers>
            <DataTrigger TargetType="dx:DXStackLayout" Binding="{Binding AllowCommonActions}" Value="False">
                <Setter Property="IsVisible" Value="False"/>
            </DataTrigger>
        </dx:DXStackLayout.Triggers>
        <dx:DXButton Content="{Binding PrimaryActionName}" Command="{Binding SecondaryActionCommand}" .../>
        <dx:DXButton Content="{Binding SecondaryActionName}" Command="{Binding PrimaryActionCommand}" .../>
    </dx:DXStackLayout>
    ```

1. Define the **Card.PrimaryAction** and **Card.SecondaryAction** commands:

    ```csharp
    namespace CollectionViewWithActionButtons.ViewModels {
        // ...
        public class Card : BindableBase {
            public Card(string title) {
                // ...
                PrimaryActionCommand = new Command(PrimaryAction);
                SecondaryActionCommand = new Command(SecondaryAction);
            }
            // ...
            public ICommand PrimaryActionCommand { get; }
            public ICommand SecondaryActionCommand { get; }
            public bool AllowCommonActions { get; set; }
            public string PrimaryActionName { get; set; }
            public string SecondaryActionName { get; set; }
            // ...
            }
            // ...
            public async void PrimaryAction() {
                await Application.Current.MainPage.DisplayAlert("Primary Action", "Click", "OK");
            }
            public async void SecondaryAction() {
                await Application.Current.MainPage.DisplayAlert("Secondary Action", "Click", "OK");
            }
            // ...
        }
    }
    ```
## Files to Look At

* [MainPage.xaml](CS/MainPage.xaml)
* [CompleteListPage.xaml](CS/Views/CompleteListPage.xaml)
* [CompleteListViewModel.cs](CS/ViewModels/CompleteListViewModel.cs)
* [MainViewModel.cs](CS/ViewModels/MainViewModel.cs)
* [Styles.xaml](CS/Resources/Styles/Styles.xaml)

## Documentation

* [Featured Scenario: Expandable Cards with Lists](https://docs.devexpress.com/MAUI/404301)
* [Featured Scenarios](https://docs.devexpress.com/MAUI/404291)
* [DevExpress Collection View for .NET MAUI](https://docs.devexpress.com/MAUI/403324/collection-view/index)
* [DXButton.Command](https://docs.devexpress.com/MAUI/DevExpress.Maui.Core.DXButtonBase.Command)
* [DXButton.CommandParameter](https://docs.devexpress.com/MAUI/DevExpress.Maui.Core.DXButtonBase.CommandParameter)

## More Examples

* [DevExpress Collection View for .NET MAUI](https://github.com/DevExpress-Examples/maui-collection-view)
* [DevExpress Mobile UI for .NET MAUI](https://github.com/DevExpress-Examples/maui-demo-app/)
<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=maui-list-card&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=maui-list-card&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
