namespace Montealegre_Sofia_RecipeDiscover
{
    public partial class MainPage : ContentPage
    {
        private  readonly ServiceApi _serviceApi = new ServiceApi();

        public MainPage()
        {
            InitializeComponent();
        }
        public async void OnSearchCompleted(object sender, EventArgs e)
        {
            string name = Search.Text;
            var recipies = await _serviceApi.GetRecipesName(name);
            FoodList.ItemsSource = recipies;
        }
        
    }

}
