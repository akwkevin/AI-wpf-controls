using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Controls.Demo.Views
{
    /// <summary>
    /// TransitionView.xaml 的交互逻辑
    /// </summary>
    public partial class TransitionView : UserControl
    {
        public TransitionView()
        {
            InitializeComponent();

            LoadTransitions(Assembly.GetAssembly(typeof(Transition)));

            // Get the default view for the transition types
            view = CollectionViewSource.GetDefaultView(TransitionTypes);

            // Set the default sort
            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            // Handle changes in currency
            view.CurrentChanged += new EventHandler(view_CurrentChanged);

            // Bind
            TransitionDS.ObjectType = null;
            TransitionTypesDS.ObjectType = null;
            TransitionTypesDS.ObjectInstance = TransitionTypes;

            // Navigate to first item
            view.MoveCurrentToFirst();
        }

        #region Constants
        /************************************************
		 * Constants
		 ***********************************************/
        private const string CellA = "CellA";
        private const string CellB = "CellB";
        #endregion // Constants

        #region Member Variables
        /************************************************
		 * Member Variables
		 ***********************************************/
        private ICollectionView view;
        private ObservableCollection<Type> transitionTypes = new ObservableCollection<Type>();
        public ObservableCollection<Type> TransitionTypes
        {
            get
            {
                return transitionTypes;
            }
        }
        #endregion // Member Variables

        /// <summary>
        /// Loads the transitions found in the specified assembly.
        /// </summary>
        /// <param name="assembly">
        /// The assembly to search for transitions in.
        /// </param>
        public void LoadTransitions(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                // Must not already exist
                if (transitionTypes.Contains(type)) { continue; }

                // Must not be abstract.
                if ((typeof(Transition).IsAssignableFrom(type)) && (!type.IsAbstract))
                {
                    transitionTypes.Add(type);
                }
            }
        }

        #region Internal Methods
        /************************************************
		 * Internal Methods
		 ***********************************************/
        /// <summary>
        /// Activates a transition and displays it.
        /// </summary>
        /// <param name="transitionType">
        /// The type of transition to activate.
        /// </param>
        private void ActivateTransition(Type transitionType)
        {
            // If no type, ignore
            if (transitionType == null) return;

            // Create the instance
            Transition transition = (Transition)Activator.CreateInstance(transitionType);

            // Bind
            TransitionDS.ObjectInstance = transition;

            // Swap cells to show transition
            SwapCell();
        }

        /// <summary>
        /// Creates an animation cell for demonstrating a transition.
        /// </summary>
        /// <param name="style">
        /// The style used to create the cell.
        /// </param>
        /// <returns>
        /// A <see cref="ContentControl"/> that represents the cell.
        /// </returns>
        private ContentControl CreateCell(Style style)
        {
            ContentControl c = new ContentControl();
            c.Style = style;
            return c;
        }

        /// <summary>
        /// Swaps the current cell, from A to B or from B to A.
        /// </summary>
        private void SwapCell()
        {
            ContentControl currentCell = (ContentControl)TransitionBox.Content;
            if ((currentCell == null) || (currentCell.Style == Resources[CellB]))
            {
                TransitionBox.Content = CreateCell((Style)Resources[CellA]);
            }
            else
            {
                TransitionBox.Content = CreateCell((Style)Resources[CellB]);
            }
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers

        private void ABButton_Click(object sender, RoutedEventArgs e)
        {
            SwapCell();
        }

        private void AButton_Click(object sender, RoutedEventArgs e)
        {
            TransitionBox.Content = CreateCell((Style)Resources[CellA]);
        }

        private void BButton_Click(object sender, RoutedEventArgs e)
        {
            TransitionBox.Content = CreateCell((Style)Resources[CellB]);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            TransitionBox.Content = null;
        }

        private void view_CurrentChanged(object sender, EventArgs e)
        {
            ActivateTransition((Type)view.CurrentItem);
        }
        #endregion // Overrides / Event Handlers

        #region Internal Properties
        /************************************************
		 * Internal Properties
		 ***********************************************/
        private ObjectDataProvider TransitionDS
        {
            get
            {
                return (ObjectDataProvider)Resources["TransitionDS"];
            }
        }

        private ObjectDataProvider TransitionTypesDS
        {
            get
            {
                return (ObjectDataProvider)Resources["TransitionTypesDS"];
            }
        }
        #endregion // Internal Properties
    }
}
