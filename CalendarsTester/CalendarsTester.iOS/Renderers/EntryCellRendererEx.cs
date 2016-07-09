using CoreGraphics;
using System;
using System.ComponentModel;
using CalendarsTester.Controls;
using CalendarsTester.iOS.Renderers;
using CalendarsTester.iOS.Extensions;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EntryCellEx), typeof(EntryCellRendererEx))]

namespace CalendarsTester.iOS.Renderers
{
    /// <summary>
    /// This is a JustDecompiled clone of the standard Xamarin.Forms EntryCellRenderer,
    /// with the exception that it supports EntryCellEx's Focus requests.
    /// </summary>
    public class EntryCellRendererEx : CellRenderer
    {
        private readonly static Color DefaultTextColor;

        static EntryCellRendererEx()
        {
            DefaultTextColor = Color.Black;
        }

        public EntryCellRendererEx()
        {
        }

        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            EntryCell entryCell = (EntryCell)item;
            EntryCellTableViewCell entryCellTableViewCell = reusableCell as EntryCellTableViewCell;
            if (entryCellTableViewCell != null)
            {
                entryCellTableViewCell.Cell.PropertyChanged -= OnCellPropertyChanged;
                entryCellTableViewCell.TextFieldTextChanged -= OnTextFieldTextChanged;
                entryCellTableViewCell.KeyboardDoneButtonPressed -= OnKeyBoardDoneButtonPressed;
            }
            else
            {
                entryCellTableViewCell = new EntryCellTableViewCell(item.GetType().FullName);
            }
            SetMyRealCell(item, entryCellTableViewCell);
            entryCellTableViewCell.Cell = item;
            entryCellTableViewCell.Cell.PropertyChanged += OnCellPropertyChanged;
            entryCellTableViewCell.TextFieldTextChanged += OnTextFieldTextChanged;
            entryCellTableViewCell.KeyboardDoneButtonPressed += OnKeyBoardDoneButtonPressed;
            base.WireUpForceUpdateSizeRequested(item, entryCellTableViewCell, tv);
            base.UpdateBackground(entryCellTableViewCell, entryCell);
            UpdateLabel(entryCellTableViewCell, entryCell);
            UpdateText(entryCellTableViewCell, entryCell);
            UpdateKeyboard(entryCellTableViewCell, entryCell);
            UpdatePlaceholder(entryCellTableViewCell, entryCell);
            UpdateLabelColor(entryCellTableViewCell, entryCell);
            UpdateHorizontalTextAlignment(entryCellTableViewCell, entryCell);
            UpdateIsEnabled(entryCellTableViewCell, entryCell);
            return entryCellTableViewCell;
        }

        private static void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            EntryCell entryCell = (EntryCell)sender;
            EntryCellTableViewCell realCell = (EntryCellTableViewCell)GetMyRealCell(entryCell);
            if (e.PropertyName == EntryCell.LabelProperty.PropertyName)
            {
                UpdateLabel(realCell, entryCell);
                return;
            }
            if (e.PropertyName == EntryCell.TextProperty.PropertyName)
            {
                UpdateText(realCell, entryCell);
                return;
            }
            if (e.PropertyName == EntryCell.PlaceholderProperty.PropertyName)
            {
                UpdatePlaceholder(realCell, entryCell);
                return;
            }
            if (e.PropertyName == EntryCell.KeyboardProperty.PropertyName)
            {
                UpdateKeyboard(realCell, entryCell);
                return;
            }
            if (e.PropertyName == EntryCell.LabelColorProperty.PropertyName)
            {
                UpdateLabelColor(realCell, entryCell);
                return;
            }
            if (e.PropertyName == EntryCell.HorizontalTextAlignmentProperty.PropertyName)
            {
                UpdateHorizontalTextAlignment(realCell, entryCell);
                return;
            }
            if (e.PropertyName == Cell.IsEnabledProperty.PropertyName)
            {
                UpdateIsEnabled(realCell, entryCell);
            }
        }

        private static void OnKeyBoardDoneButtonPressed(object sender, EventArgs e)
        {
            // We don't have access to SendCompleted...
            //
            //((EntryCell)((EntryCellTableViewCell)sender).Cell).SendCompleted();
        }

        private static void OnTextFieldTextChanged(object sender, EventArgs eventArgs)
        {
            EntryCellTableViewCell text = (EntryCellTableViewCell)sender;
            ((EntryCell)text.Cell).Text = text.TextField.Text;
        }

        private static void UpdateHorizontalTextAlignment(EntryCellTableViewCell cell, EntryCell entryCell)
        {
            cell.TextField.TextAlignment = entryCell.HorizontalTextAlignment.ToNativeTextAlignment();
        }

        private static void UpdateIsEnabled(EntryCellTableViewCell cell, EntryCell entryCell)
        {
            cell.UserInteractionEnabled = entryCell.IsEnabled;
            cell.TextLabel.Enabled = entryCell.IsEnabled;
            cell.DetailTextLabel.Enabled = entryCell.IsEnabled;
            cell.TextField.Enabled = entryCell.IsEnabled;
        }

        private static void UpdateKeyboard(EntryCellTableViewCell cell, EntryCell entryCell)
        {
            cell.TextField.ApplyKeyboard(entryCell.Keyboard);
        }

        private static void UpdateLabel(EntryCellTableViewCell cell, EntryCell entryCell)
        {
            cell.TextLabel.Text = entryCell.Label;
        }

        private static void UpdateLabelColor(EntryCellTableViewCell cell, EntryCell entryCell)
        {
            cell.TextLabel.TextColor = entryCell.LabelColor.ToUIColor(DefaultTextColor);
        }

        private static void UpdatePlaceholder(EntryCellTableViewCell cell, EntryCell entryCell)
        {
            cell.TextField.Placeholder = entryCell.Placeholder;
        }

        private static void UpdateText(EntryCellTableViewCell cell, EntryCell entryCell)
        {
            if (cell.TextField.Text == entryCell.Text)
            {
                return;
            }
            cell.TextField.Text = entryCell.Text;
        }


        // Bindable attached property for MyRealCell
        // - this is a copy of CellRenderer.RealCell, which we don't have access to :(
        //
        public static readonly BindableProperty MyRealCellProperty =
            BindableProperty.CreateAttached("MyRealCell", typeof(UITableViewCell), typeof(EntryCellRendererEx), null, BindingMode.OneWay);
          //BindableProperty.CreateAttached<EntryCellRendererEx, UITableViewCell>(child => GetMyRealCell(child), null);

        public static UITableViewCell GetMyRealCell(BindableObject child)
        {
            return (UITableViewCell)child.GetValue(MyRealCellProperty);
        }

        public static void SetMyRealCell(BindableObject child, UITableViewCell value)
        {
            child.SetValue(MyRealCellProperty, value);
        }


        private class EntryCellTableViewCell : CellTableViewCell
        {
            public UITextField TextField
            {
                get;
                private set;
            }

            public EntryCellTableViewCell(string cellName) : base(UITableViewCellStyle.Value1, cellName)
            {
                UITextField uITextField = new UITextField(new CGRect(0, 0, 100, 30));
                uITextField.BorderStyle = 0;
                TextField = uITextField;
                TextField.EditingChanged += TextFieldOnEditingChanged;
                TextField.ShouldReturn = OnShouldReturn;
                ContentView.AddSubview(TextField);
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                CGRect labelFrame = TextLabel.Frame;
                nfloat offsetForLabel = (nfloat)(Math.Round(Math.Max(Frame.Width * 0.3, (labelFrame.Right + 10))));
                UITextField textField = this.TextField;
                nfloat fieldHeight = (Frame.Height - 30) / 2;

                // Don't allocate space for an empty label
                // 
                if (string.IsNullOrEmpty(TextLabel.Text))
                {
                    textField.Frame = new CGRect(labelFrame.Left, fieldHeight, Frame.Width - labelFrame.Left, 30);
                }
                else
                {
                    textField.Frame = new CGRect(offsetForLabel, fieldHeight, (Frame.Width - labelFrame.Left) - offsetForLabel, 30);
                }
                TextField.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            }

            private bool OnShouldReturn(UITextField view)
            {
                EventHandler eventHandler = KeyboardDoneButtonPressed;
                if (eventHandler != null)
                {
                    eventHandler.Invoke(this, EventArgs.Empty);
                }
                TextField.ResignFirstResponder();
                return true;
            }

            private void TextFieldOnEditingChanged(object sender, EventArgs eventArgs)
            {
                EventHandler eventHandler = TextFieldTextChanged;
                if (eventHandler != null)
                {
                    eventHandler.Invoke(this, EventArgs.Empty);
                }
            }

            public event EventHandler KeyboardDoneButtonPressed;

            public event EventHandler TextFieldTextChanged;
        }
    }
}