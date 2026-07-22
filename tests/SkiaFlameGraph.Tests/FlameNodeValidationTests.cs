using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Tests;

public class FlameNodeValidationTests
{
    [Fact]
    public void Validate_ValidNode_ReturnsEmptyList()
    {
        // Arrange
        var node = new FlameNode("validMethod")
        {
            Value = 100,
            Depth = 0,
            File = "test.cs",
            Line = 42
        };
        node.Children.Add(new FlameNode("childMethod") { Value = 50, Depth = 1 });

        // Act
        var result = node.Validate();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Validate_NullName_ReturnsError()
    {
        // Arrange
        var node = new FlameNode(null!)
        {
            Value = 100,
            Depth = 0
        };

        // Act
        var result = node.Validate();

        // Assert
        Assert.Single(result);
        Assert.Contains("Name cannot be null or whitespace.", result);
    }

    [Fact]
    public void Validate_WhitespaceName_ReturnsError()
    {
        // Arrange
        var node = new FlameNode("   ")
        {
            Value = 100,
            Depth = 0
        };

        // Act
        var result = node.Validate();

        // Assert
        Assert.Single(result);
        Assert.Contains("Name cannot be null or whitespace.", result);
    }

    [Fact]
    public void Validate_NegativeValue_ReturnsError()
    {
        // Arrange
        var node = new FlameNode("method")
        {
            Value = -1,
            Depth = 0
        };

        // Act
        var result = node.Validate();

        // Assert
        Assert.Single(result);
        Assert.Contains("Value cannot be negative.", result);
    }

    [Fact]
    public void Validate_NegativeDepth_ReturnsError()
    {
        // Arrange
        var node = new FlameNode("method")
        {
            Value = 100,
            Depth = -1
        };

        // Act
        var result = node.Validate();

        // Assert
        Assert.Single(result);
        Assert.Contains("Depth cannot be negative.", result);
    }

    [Fact]
    public void Validate_NullChildren_ReturnsError()
    {
        // Arrange - Children is read-only, so we can't assign null
        // This test is not possible to write as written since Children is read-only
        // We'll test the null child in collection scenario instead
        var node = new FlameNode("method")
        {
            Value = 100,
            Depth = 0
        };
        node.Children.Add(null!); // Force null child

        // Act
        var result = node.Validate();

        // Assert
        Assert.Single(result);
        Assert.Contains("Children collection contains a null element.", result);
    }

    [Fact]
    public void Validate_NullChildInCollection_ReturnsError()
    {
        // Arrange
        var node = new FlameNode("method")
        {
            Value = 100,
            Depth = 0
        };
        node.Children.Add(null!); // Force null child
        node.Children.Add(new FlameNode("validChild") { Value = 50, Depth = 1 });

        // Act
        var result = node.Validate();

        // Assert
        Assert.Single(result);
        Assert.Contains("Children collection contains a null element.", result);
    }

    [Fact]
    public void Validate_ZeroOrNegativeLine_ReturnsError()
    {
        // Arrange
        var node = new FlameNode("method")
        {
            Value = 100,
            Depth = 0,
            Line = 0
        };

        // Act
        var result = node.Validate();

        // Assert
        Assert.Single(result);
        Assert.Contains("Line, if specified, must be a positive integer.", result);
    }

    [Fact]
    public void Validate_EmptyFile_IsValid()
    {
        // Arrange
        var node = new FlameNode("method")
        {
            Value = 100,
            Depth = 0,
            File = ""
        };

        // Act
        var result = node.Validate();

        // Assert - Empty file is valid (null/whitespace check passes)
        Assert.Empty(result);
    }

    [Fact]
    public void Validate_NullNode_ThrowsArgumentNullException()
    {
        // Arrange
        FlameNode? node = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => node!.Validate());
    }

    [Fact]
    public void IsValid_ValidNode_ReturnsTrue()
    {
        // Arrange
        var node = new FlameNode("validMethod")
        {
            Value = 100,
            Depth = 0
        };

        // Act
        var result = node.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_InvalidNode_ReturnsFalse()
    {
        // Arrange
        var node = new FlameNode(null!)
        {
            Value = 100,
            Depth = 0
        };

        // Act
        var result = node.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_NullNode_ReturnsFalse()
    {
        // Arrange
        FlameNode? node = null;

        // Act
        var result = node.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void EnsureValid_ValidNode_DoesNotThrow()
    {
        // Arrange
        var node = new FlameNode("validMethod")
        {
            Value = 100,
            Depth = 0
        };

        // Act & Assert
        var exception = Record.Exception(() => node.EnsureValid());
        Assert.Null(exception);
    }

    [Fact]
    public void EnsureValid_NullNode_ThrowsArgumentNullException()
    {
        // Arrange
        FlameNode? node = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => node!.EnsureValid());
    }

    [Fact]
    public void EnsureValid_InvalidNode_ThrowsArgumentException()
    {
        // Arrange
        var node = new FlameNode(null!)
        {
            Value = 100,
            Depth = 0
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => node.EnsureValid());
        Assert.Contains("The FlameNode is not valid.", exception.Message);
    }

    [Fact]
    public void Validate_MultipleProblems_ReturnsAllErrors()
    {
        // Arrange
        var node = new FlameNode(null!)
        {
            Value = -1,
            Depth = -1
        };
        node.Children.Add(null!);

        // Act
        var result = node.Validate();

        // Assert
        Assert.Equal(4, result.Count);
        Assert.Contains("Name cannot be null or whitespace.", result);
        Assert.Contains("Value cannot be negative.", result);
        Assert.Contains("Depth cannot be negative.", result);
        Assert.Contains("Children collection contains a null element.", result);
    }

    [Fact]
    public void Validate_EmptyChildrenCollection_IsValid()
    {
        // Arrange
        var node = new FlameNode("method")
        {
            Value = 100,
            Depth = 0
        };
        // Children is initialized as empty list in constructor, so no assignment needed

        // Act
        var result = node.Validate();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Validate_PositiveLine_IsValid()
    {
        // Arrange
        var node = new FlameNode("method")
        {
            Value = 100,
            Depth = 0,
            Line = 1
        };

        // Act
        var result = node.Validate();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Validate_ValidNodeWithFileAndLine_IsValid()
    {
        // Arrange
        var node = new FlameNode("method")
        {
            Value = 100,
            Depth = 0,
            File = "Program.cs",
            Line = 42
        };

        // Act
        var result = node.Validate();

        // Assert
        Assert.Empty(result);
    }
}