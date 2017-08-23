using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ONP_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void calculateBtn_Click(object sender, RoutedEventArgs e)
        {
            resultText.Text = calculateOnp(inputText.Text).ToString();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                resultText.Text = calculateOnp(inputText.Text).ToString();
            }
        }

        public static string toOnp(string sInput)
        {
            Stack<char> charStack = new Stack<char>();
            string output = "";

            try
            {
                if (!String.IsNullOrEmpty(sInput))
                {
                    for (int i = 0; i < sInput.Length; i++)
                    {
                        if (Char.IsNumber(sInput[i]) || sInput[i] == '.' || sInput[i] == ',')
                        {
                            if (i == 0)
                                output = output + sInput[i];
                            else if (Char.IsNumber(sInput[i - 1]) || sInput[i - 1] == '.' || sInput[i - 1] == ',')
                                output = output + sInput[i];
                            else
                                output = output + ' ' + sInput[i];
                        }

                        else if (sInput[i] == '(')
                        {
                            charStack.Push(sInput[i]);
                        }

                        else if (sInput[i] == ')')
                        {
                            while (charStack.Peek() != '(')
                            {
                                output = output + ' ' + charStack.Pop();
                            }
                            charStack.Pop();
                        }

                        else if (sInput[i] == '+' || sInput[i] == '-' || sInput[i] == '*' || sInput[i] == '/' || sInput[i] == '%' || sInput[i] == '^')
                        {
                            if (charStack.Count == 0 || Priority(sInput[i]) > Priority(charStack.Peek()))
                            {
                                charStack.Push(sInput[i]);
                            }
                            else
                            {
                                while (Priority(charStack.Peek()) >= Priority(sInput[i]))
                                {
                                    output = output + ' ' + charStack.Pop();
                                    if (charStack.Count == 0)
                                        break;
                                }
                                charStack.Push(sInput[i]);
                            }
                        }
                    }
                    while (charStack.Count != 0)
                    {
                        output = output + ' ' + charStack.Pop();
                    }
                }
                else
                {
                    MessageBox.Show("Błędne wejście!", "Calculator ONP", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błędne wejście! " + ex.Message, "Calculator ONP", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
            return output.Replace(',', '.').Trim();
        }

        public static double calculateOnp(string input)
        {
            input = toOnp(input);
            if (String.IsNullOrEmpty(input))
            {
                return 0.0;
            }
            
            var splitedInput = input.Split();
            double a, b;
            Stack<double> resultStack = new Stack<double>();

            for (int i = 0; i < splitedInput.Length; i++)
            {
                if (Double.TryParse(splitedInput[i], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out a))
                {
                    resultStack.Push(a);
                }
                else
                {
                    try
                    {
                        b = resultStack.Pop();
                        a = resultStack.Pop();

                        switch (splitedInput[i])
                        {
                            case "+": resultStack.Push(a + b);
                                break;
                            case "-": resultStack.Push(a - b);
                                break;
                            case "*": resultStack.Push(a * b);
                                break;
                            case "/": resultStack.Push(a / b);
                                break;
                            case "^": resultStack.Push(Math.Pow(a, b));
                                break;
                            case "%": resultStack.Push(a % b);
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błędne wejście! " + ex.Message, "Calculator ONP", MessageBoxButton.OK, MessageBoxImage.Warning);
                    } 
                    
                }
            }
            return (resultStack.Count > 0) ? resultStack.Peek() : 0.0;
        }
        
        public static int Priority(char input)
        {
            if (input == '(')
                return 0;
            else if (input == '+' || input == '-' || input == ')')
                return 1;
            else if (input == '*' || input == '/' || input == '%')
                return 2;
            else if (input == '^')
                return 3;
            else
                return 0;
        }
    }
}
