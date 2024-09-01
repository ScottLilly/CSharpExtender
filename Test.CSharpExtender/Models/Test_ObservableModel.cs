using CSharpExtender.Models;

namespace Test.CSharpExtender.Models
{
    public class Test_ObservableModel
    {
        [Fact]
        public void Test_PropertyChangedShouldFireWhenPropertyIsSet()
        {
            var model = new TestModel();
            var propertyChangedFired = false;
            var propertyName = string.Empty;

            model.PropertyChanged += (sender, args) =>
            {
                propertyChangedFired = true;
                propertyName = args.PropertyName;
            };

            model.Name = "Test Name";

            Assert.True(propertyChangedFired);
            Assert.Equal("Name", propertyName);
        }

        [Fact]
        public void Test_PropertyChangedShouldNotFireWhenPropertyIsSetToSameValue()
        {
            var model = new TestModel();
            model.Name = "Initial Name";
            var propertyChangedFired = false;

            model.PropertyChanged += (sender, args) =>
            {
                propertyChangedFired = true;
            };

            model.Name = "Initial Name";

            Assert.False(propertyChangedFired);
        }

        #region Class for unit tests

        public class TestModel : ObservableModel
        {
            private int _id;
            private string _name = string.Empty;

            public int Id 
            {
                get => _id;
                set
                {
                    SetProperty(ref _id, value);
                }
            }

            public string Name 
            { 
                get => _name;
                set
                {
                    SetProperty(ref _name, value);
                }
            }
        }

        #endregion
    }
}
