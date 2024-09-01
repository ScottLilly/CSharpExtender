using CSharpExtender.Models;

namespace Test.CSharpExtender.Models
{
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
    }

    #endregion
}
