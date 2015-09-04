using System.Windows;
using System.Windows.Controls;

namespace BlogExporter.Shell
{

    //To Demo How to 
    public  static class ScrollToEndBehavior
    {
        public static readonly DependencyProperty OnTextChangedProperty =
                    DependencyProperty.RegisterAttached(
                    "OnTextChanged",
                    typeof(bool),
                    typeof(ScrollToEndBehavior),
                    new UIPropertyMetadata(false, OnTextChanged)
                    );

        public static bool GetOnTextChanged(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(OnTextChangedProperty);
        }

        public static void SetOnTextChanged(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(OnTextChangedProperty, value);
        }

        private static void OnTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var textBox = dependencyObject as TextBox;
            var newValue = (bool)e.NewValue;

            if (textBox == null || (bool)e.OldValue == newValue)
            {
                return;
            }

            TextChangedEventHandler handler = (object sender, TextChangedEventArgs args) =>
                ((TextBox)sender).ScrollToEnd();

            if (newValue)
            {
                textBox.TextChanged += handler;
            }
            else
            {
                textBox.TextChanged -= handler;
            }
        }
    }
}