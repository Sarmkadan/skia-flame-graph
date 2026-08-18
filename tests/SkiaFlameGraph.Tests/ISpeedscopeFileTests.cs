namespace SkiaFlameGraph.Tests;

public interface ISpeedscopeFileTests
{
    void Deserialize_WithValidJson_ReturnsPopulatedFile();
    void Deserialize_WithMinimalJson_ReturnsValidFile();
    void Schema_GetAndSet_ReturnsExpectedValue();
    void Schema_SetToNull_ReturnsNull();
    void Name_GetAndSet_ReturnsExpectedValue();
    void Name_SetToNull_ReturnsNull();
    void Exporter_GetAndSet_ReturnsExpectedValue();
    void Exporter_SetToNull_ReturnsNull();
    void Shared_Get_ReturnsInitializedInstance();
    void Shared_Set_ReplacesInstance();
    void Profiles_Get_ReturnsInitializedList();
    void Profiles_AddItems_ListContainsItems();
    void Profiles_Clear_ListIsEmpty();
    void Shared_Frames_Get_ReturnsInitializedList();
    void Shared_Frames_AddItems_ListContainsItems();
    void Frame_Name_Get_ReturnsEmptyStringByDefault();
    void Frame_Name_Set_ReturnsExpectedValue();
    void Frame_File_CanBeNull();
    void Frame_Line_CanBeNull();
    void Frame_Col_CanBeNull();
}
