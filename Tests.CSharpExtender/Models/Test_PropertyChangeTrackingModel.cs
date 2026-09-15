using CSharpExtender.Models;

namespace Tests.CSharpExtender.Models;

public class Test_PropertyChangeTrackingModel
{
    [Fact]
    public void Test_IsChanged_InitiallyFalse()
    {
        var model = new TestModel();

        Assert.False(model.IsChanged);
    }

    [Fact]
    public void Test_IsChanged_TrueAfterPropertyChange()
    {
        var model = new TestModel();
        model.Id = 1;

        Assert.True(model.IsChanged);
    }

    [Fact]
    public void Test_PropertyChangeLog_EmptyInitially()
    {
        var model = new TestModel();

        Assert.Empty(model.PropertyChangeLog);
    }

    [Fact]
    public void Test_PropertyChangeLog_LogsChanges()
    {
        var model = new TestModel();
        model.Id = 1;
        model.Name = "Test";

        Assert.Equal(2, model.PropertyChangeLog.Count);
        Assert.Equal("Id", model.PropertyChangeLog[0].PropertyName);
        Assert.Equal(1, model.PropertyChangeLog[0].NewValue);
        Assert.Equal("Name", model.PropertyChangeLog[1].PropertyName);
        Assert.Equal("Test", model.PropertyChangeLog[1].NewValue);
    }

    [Fact]
    public void Test_PropertyChangeLog_DoesNotLogUnchangedProperties()
    {
        var model = new TestModel();
        model.Id = 1;
        model.Id = 1; // Setting to the same value

        Assert.Single(model.PropertyChangeLog);
        Assert.Equal("Id", model.PropertyChangeLog[0].PropertyName);
        Assert.Equal(1, model.PropertyChangeLog[0].NewValue);
    }

    [Fact]
    public void Test_AcceptChanges_ClearsChangeLogAndResetsIsChanged()
    {
        var model = new TestModel();
        model.Id = 1;
        model.Name = "Test";

        Assert.True(model.IsChanged);
        Assert.Equal(2, model.PropertyChangeLog.Count);

        model.AcceptChanges();

        Assert.False(model.IsChanged);
        Assert.Empty(model.PropertyChangeLog);
    }

    [Fact]
    public void Test_IsChanged_FalseAfterAcceptChanges()
    {
        var model = new TestModel();
        model.Id = 1;
        Assert.True(model.IsChanged);

        model.AcceptChanges();
        Assert.False(model.IsChanged);
    }

    [Fact]
    public void Test_PropertyChangeLog_OrderOfChangesPreserved()
    {
        var model = new TestModel();
        model.Id = 1;
        model.Name = "Test";
        model.Id = 2;

        var propertyNames = model.PropertyChangeLog.Select(log => log.PropertyName).ToList();
        Assert.Equal(new[] { "Id", "Name", "Id" }, propertyNames);
    }

    [Fact]
    public void Test_IsChanged_RemainsTrueForMultipleChanges()
    {
        var model = new TestModel();
        model.Id = 1;
        Assert.True(model.IsChanged);

        model.Name = "Test";
        Assert.True(model.IsChanged);
    }

    [Fact]
    public void Test_PropertyChangeLog_StoresCorrectValues()
    {
        var model = new TestModel();
        model.Id = 1;
        model.Id = 2;
        model.Name = "Test";

        Assert.Equal(3, model.PropertyChangeLog.Count);
        Assert.Equal(1, model.PropertyChangeLog[0].NewValue);
        Assert.Equal(2, model.PropertyChangeLog[1].NewValue);
        Assert.Equal("Test", model.PropertyChangeLog[2].NewValue);
    }

    [Fact]
    public void Test_ChangeDateTime_IsUtc()
    {
        var before = DateTime.UtcNow;

        var model = new TestModel();
        model.Id = 1;

        var after = DateTime.UtcNow;
        var stamped = model.PropertyChangeLog[0].ChangeDateTime;

        Assert.Equal(DateTimeKind.Utc, stamped.Kind);
        Assert.InRange(stamped, before, after);
    }

    [Fact]
    public void Test_ChangeDateTime_OrdersEntriesByWhenTheyWereWritten()
    {
        // The point of UTC here: a local timestamp cannot order two entries written
        // in the hour that repeats every autumn
        var model = new TestModel();

        model.Id = 1;
        model.Name = "Test";

        var first = model.PropertyChangeLog[0].ChangeDateTime;
        var second = model.PropertyChangeLog[1].ChangeDateTime;

        Assert.True(second >= first);
    }

    [Fact]
    public void Test_PropertyChangedLog_NullPropertyName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new PropertyChangedLog(null!, "value"));
    }

    [Fact]
    public void Test_SetProperty_NullPropertyName_NotifiesButLogsNothing()
    {
        // A null name is INotifyPropertyChanged's "every property changed", which the
        // log has no single property to record it against
        var model = new TestModel();
        var raised = new List<string?>();

        model.PropertyChanged += (_, args) => raised.Add(args.PropertyName);

        Assert.True(model.SetNameWithNoPropertyName("Test"));
        Assert.Equal("Test", model.Name);
        Assert.Contains(null, raised);
        Assert.Empty(model.PropertyChangeLog);
        Assert.False(model.IsChanged);
    }
}

#region Class for unit tests

public class TestModel : PropertyChangeTrackingModel
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

    // SetProperty is protected, and [CallerMemberName] fills the name in for every
    // normal call, so reaching the null case takes a deliberate one
    public bool SetNameWithNoPropertyName(string value) =>
        SetProperty(ref _name, value, null);
}

#endregion
