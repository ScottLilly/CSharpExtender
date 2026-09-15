using CSharpExtender.Models;
using System.ComponentModel;

namespace Test.CSharpExtender.Models;

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

    [Fact]
    public void SetProperty_ValueTypeSetToTheSameValue_DoesNotFire()
    {
        // The comparison runs through EqualityComparer rather than the static
        // object.Equals, so this has to keep answering the same way for a struct
        var model = new TestModel { Id = 7 };
        var fired = false;

        model.PropertyChanged += (_, _) => fired = true;

        model.Id = 7;

        Assert.False(fired);

        model.Id = 8;

        Assert.True(fired);
    }

    [Fact]
    public void SetProperty_NullableAndDefaultValues_AreComparedCorrectly()
    {
        var model = new TestModel();
        var raised = new List<string?>();

        model.PropertyChanged += (_, args) => raised.Add(args.PropertyName);

        // Already the default, so nothing changes
        model.Id = 0;
        model.Name = string.Empty;

        Assert.Empty(raised);

        model.Description = null;

        Assert.Empty(raised);

        model.Description = "set";

        Assert.Equal(["Description"], raised);
    }

    [Fact]
    public void PropertyChanged_EveryRaise_NamesTheRightProperty()
    {
        var model = new TestModel();
        var raised = new List<string?>();

        model.PropertyChanged += (_, args) => raised.Add(args.PropertyName);

        model.Name = "first";
        model.Name = "second";
        model.Id = 1;

        Assert.Equal(["Name", "Name", "Id"], raised);
    }

    #region Class for unit tests

    public class TestModel : ObservableModel
    {
        private int _id;
        private string _name = string.Empty;
        private string? _description;

        public string? Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
            }
        }

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
