namespace LurraldeOrdezkaritzak.ItemViews
{
    public partial class EskaeraEgoitzaEskatu : ContentView
    {
        public event EventHandler EskatuButtonClicked;

        public EskaeraEgoitzaEskatu()
        {
            InitializeComponent();
        }

        private void OnEskatuButtonClickedAsync(object sender, EventArgs e)
        {
            EskatuButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}