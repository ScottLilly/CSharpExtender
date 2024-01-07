using CSharpExtender.Services;

namespace Test.CSharpExtender.Services;

public class Test_RngCreator
{
    [Fact]
    public void TestGetNumberBetween()
    {
        int min = 2;
        int max = 12;
        bool has2 = false;
        bool has12 = false;

        // Run 1000 times, to ensure we don't get a value outside of the range
        for (int i = 0; i < 1000; i++)
        {
            int result = RngCreator.GetNumberBetween(min, max);

            // Verify that the result is between 2 and 12
            Assert.True(result >= min && result <= max);

            // Check if 2 and 12 were generated at least once
            if (result == 2)
            {
                has2 = true;
            }
            else if (result == 12)
            {
                has12 = true;
            }
        }

        // Ensure that at least one 2 and one 12 were generated
        Assert.True(has2);
        Assert.True(has12);
    }
}