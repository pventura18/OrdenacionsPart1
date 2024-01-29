using System;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace OrdenacionsPart1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributs
        Random r = new Random();
        private bool stopSorting;
        List<Rectangle> rectangles;
        List<int> numeros;
        SolidColorBrush correcteBrush = new SolidColorBrush(Colors.Green);
        SolidColorBrush incorrecteBrush = new SolidColorBrush(Colors.DarkRed);
        SolidColorBrush intercanviaBrush = new SolidColorBrush(Colors.Yellow);
        SolidColorBrush fonsBrush = new SolidColorBrush(Colors.LightGray);
        SolidColorBrush brilloMarc = new SolidColorBrush(Colors.Black);

        #endregion

        #region Constructor
        public MainWindow()
        {
            rectangles = new List<Rectangle>();
            numeros = new List<int>();

            InitializeComponent();
            canvas.Background = fonsBrush;
        }

        #endregion

        #region Genera
        /// <summary>
        /// Genera les diferents figures en funció de la ordenació indicada i les pinta al canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenera_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            numeros = new List<int>();
            rectangles = new List<Rectangle>();
            if (cbInvertit.IsChecked == true) GeneraInvertit(); //Generar amb ordre invertit
            else if (cbAleatori.IsChecked == true) GeneraAleatori();//Generar amb ordre aleatori
            else GeneraOrdenat(); //Generar amb ordre creixent
        }

        /// <summary>
        /// Genera una ordenació ordenada dels valors
        /// </summary>
        private void GeneraOrdenat()
        {
            for (int i = 0; i < (int)iudNElements.Value; i++)
            {
                numeros.Add(i + 1);
                rectangles.Add(GeneraFigura(i + 1, i + 1));
                PintaRectangle(i);
            }
        }

        /// <summary>
        /// Genera una ordenació invertida dels valors
        /// </summary>
        private void GeneraInvertit()
        {
            int valor = (int)iudNElements.Value;
            for(int i = 0; i < (int)iudNElements.Value; i++)
            {
                numeros.Add(valor);
                rectangles.Add(GeneraFigura(i + 1, valor));
                PintaRectangle(i);
                valor--;
            }
            
        }

        /// <summary>
        /// Genera una ordenació aleatoria dels valor
        /// </summary>
        private void GeneraAleatori()
        {
            for (int i = 0; i < (int)iudNElements.Value; i++)
            {
                numeros.Add(i + 1);
            }

            for (int i = numeros.Count - 1; i > 0; i--)
            {
                int j = r.Next(0, i + 1);
                int temp = numeros[i];
                numeros[i] = numeros[j];
                numeros[j] = temp;
            }

            for (int i = 0; i < numeros.Count; i++)
            {
                rectangles.Add(GeneraFigura(i + 1, numeros[i]));
                PintaRectangle(i);
            }
        }

        /// <summary>
        /// Genera una figura que pertany a un valor de la llista i el pinta al canvas
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        private Rectangle GeneraFigura(int pos, int valor)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = canvas.Width / (int)iudNElements.Value;
            if(cbTipusFigura.Text == "Barres")
            {
                rectangle.Height = valor * (canvas.Height / (int)iudNElements.Value);
                rectangle.RadiusX = (double)iudRadiColumnes.Value;
                rectangle.RadiusY = (double)iudRadiColumnes.Value;
            }
            else if (cbTipusFigura.Text == "Punts")
            {
                rectangle.Height = rectangle.Width;
                rectangle.RadiusX = 100;
                rectangle.RadiusY = 100;
            }
                
            rectangle.StrokeThickness = (double)iudGruixMarc.Value;
            rectangle.Fill = correcteBrush;
            rectangle.Stroke = brilloMarc;
            

            double leftPosition = rectangle.Width * (pos - 1);
            double topPosition = canvas.ActualHeight - (valor * (canvas.Height / (int)iudNElements.Value));

            Canvas.SetLeft(rectangle, leftPosition);
            Canvas.SetTop(rectangle, topPosition);

            canvas.Children.Add(rectangle);

            return rectangle;
        }
        #endregion

        #region Ordenació

        /// <summary>
        /// Mètode que ordena els valors de la llista en funció de l'algoritme seleccionat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOrdena_Click(object sender, RoutedEventArgs e)
        {
            stopSorting = false;
            if(cbTipusAlgoritme.Text == "Bubble Sort")
            {
                DoBubbleSort();
            }
            else if(cbTipusAlgoritme.Text == "Insertion Sort")
            {
                DoInsertionSort();
            }
            else if(cbTipusAlgoritme.Text == "Quick Sort")
            {
                DoQuickSort(numeros, 0, numeros.Count - 1);
            }
        }

        /// <summary>
        /// Mètode que atura l'ordenació
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAtura_Click(object sender, RoutedEventArgs e)
        {
            stopSorting = true;
        }

        #region Algoritmes
        /// <summary>
        /// Ordena utilitzant el mètode Bubble Sort
        /// </summary>
        private void DoBubbleSort()
        {
            for (int i = 1; i < numeros.Count && !stopSorting; i++)
            {
                for (int j = 0; j < numeros.Count - i && !stopSorting; j++)
                {
                    if (numeros[j] > numeros[j + 1])
                    {
                        Intercanvia(j, j + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Ordena utilitzant el mètode Insertion Sort
        /// </summary>
        private void DoInsertionSort()
        {
            for (int i = 1; i < numeros.Count && !stopSorting; i++)
            {
                int j = i;
                while (j > 0 && numeros[j - 1] > numeros[j] && !stopSorting)
                {
                    Intercanvia(j - 1, j);
                    j--;
                }
            }
        }


        /// <summary>
        /// Ordena utilitzant el mètode Quick Sort
        /// </summary>
        /// <param name="numeros"></param>
        /// <param name="leftIndex"></param>
        /// <param name="rightIndex"></param>
        private void DoQuickSort(List<int> numeros, int leftIndex, int rightIndex)
        {
            if (stopSorting)
                return;

            int i = leftIndex;
            int j = rightIndex;
            int pivot = numeros[leftIndex];
            while (i <= j && !stopSorting)
            {
                while (numeros[i] < pivot)
                {
                    i++;
                }

                while (numeros[j] > pivot)
                {
                    j--;
                }
                if (i <= j)
                {
                    Intercanvia(i, j);

                    i++;
                    j--;
                }
            }

            if (leftIndex < j && !stopSorting)
            {
                DoQuickSort(numeros, leftIndex, j);
            }

            if (i < rightIndex && !stopSorting)
            {
                DoQuickSort(numeros, i, rightIndex);
            }
        }

        #endregion
        #endregion

        #region Colors
        /// <summary>
        /// Method that when executed changes the correct color brush
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cpCorrecte_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            correcteBrush.Color = cpCorrecte.SelectedColor.Value;

        }

        /// <summary>
        /// Method that when executed changes the incorrect color brush
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cpIncorrecte_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            incorrecteBrush.Color = cpIncorrecte.SelectedColor.Value;
        }

        /// <summary>
        /// Method that when executed changes the intercanvia color brush
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cpIntercanviar_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            intercanviaBrush.Color = cpIntercanviar.SelectedColor.Value;
        }

        /// <summary>
        /// Method that when executed changes the background color brush
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cpFons_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fonsBrush.Color = cpFons.SelectedColor.Value;
        }

        private void cpBrilloMarc_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            brilloMarc.Color = cpBrilloMarc.SelectedColor.Value;
        }

        private void PintaRectangle(int pos)
        {
            if (cbTipusFigura.Text == "Barres")
            {
                rectangles[pos].Height = numeros[pos] * (canvas.Height / (int)iudNElements.Value);
            }
            else if (cbTipusFigura.Text == "Punts")
            {
                rectangles[pos].Height = rectangles[pos].Width;
            }

            Canvas.SetTop(rectangles[pos], canvas.ActualHeight - (numeros[pos] * (canvas.Height / (int)iudNElements.Value)));

            if (numeros[pos] == numeros.IndexOf(numeros[pos]) + 1)
            {
                rectangles[pos].Fill = correcteBrush;
            }
            else
            {
                rectangles[pos].Fill = incorrecteBrush;
            }
        }

        #endregion

        #region Radi i Grux Marc
        /// <summary>
        /// Metode que al canviar el gruix del marc canvia el gruix dels rectangles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void iudGruixMarc_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            foreach (Rectangle rectangle in rectangles)
            {
                rectangle.StrokeThickness = (double)iudGruixMarc.Value;
            }
        }

        /// <summary>
        /// Metode que al canviar el radi dels rectangles canvia el radi dels rectangles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void iudRadiColumnes_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(cbTipusFigura != null && cbTipusFigura.Text == "Barres")
            {
                foreach (Rectangle rectangle in rectangles)
                {
                    rectangle.RadiusX = (double)iudRadiColumnes.Value;
                    rectangle.RadiusY = (double)iudRadiColumnes.Value;
                }
            }
            
        }

        #endregion

        #region Intercanvia
        /// <summary>
        /// Mètode que intercanvia dos valors de la llista i els pinta al canvas
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        private void Intercanvia(int pos1, int pos2)
        {
            int temp = numeros[pos1];
            numeros[pos1] = numeros[pos2];
            numeros[pos2] = temp;
            if (cbMarcaIntercanvis.IsChecked == true)
            {
                rectangles[pos1].Fill = intercanviaBrush;
                rectangles[pos2].Fill = intercanviaBrush;
            }

            //Posar les animacions aqui

            Espera((double)iudTempsPausa.Value);
               
            PintaRectangle(pos1);
            PintaRectangle(pos2);

            DoEvents();
        }

        #endregion

        #region Tipus de figura
        

        /// <summary>
        /// Mètode que al canviar el tipus de figura de punt a rectangle i a l'inversa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbTipusFigura_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTipusFigura.Text != "Barres")
            {
                for (int pos = 0; pos < rectangles.Count; pos++)
                {
                    rectangles[pos].Height = numeros[pos] * (canvas.Height / (int)iudNElements.Value);
                    rectangles[pos].RadiusX = (double)iudRadiColumnes.Value;
                    rectangles[pos].RadiusY = (double)iudRadiColumnes.Value;
                }
            }
            else if (cbTipusFigura.Text != "Punts")
            {
                for (int pos = 0; pos < rectangles.Count; pos++)
                {
                    rectangles[pos].Height = rectangles[pos].Width;
                    rectangles[pos].RadiusX = 100;
                    rectangles[pos].RadiusY = 100;
                }
            }
        }

        #endregion

        #region Threads
        Thread thread;
        private void Espera(double milliseconds)
        {
            var frame = new DispatcherFrame();
            thread = new Thread((ThreadStart)(() =>
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(milliseconds));
                frame.Continue = false;
            }));
            thread.Start();
            Dispatcher.PushFrame(frame);
        }
        static Action action;
        public static void DoEvents()
        {
            action = new Action(delegate { });
            Application.Current?.Dispatcher?.Invoke(
               System.Windows.Threading.DispatcherPriority.Background,
               action);
        }
        protected override void OnClosed(EventArgs e)
        {
            Application.Current.Dispatcher.InvokeShutdown();
            thread?.Abort();
            base.OnClosed(e);
        }
        #endregion
    }
}
