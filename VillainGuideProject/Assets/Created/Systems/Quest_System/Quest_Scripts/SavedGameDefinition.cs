using UnityEngine;

// Bake the Unity asset GUID so player builds can resolve progress after an asset is moved or renamed.
public abstract class SavedGameDefinition : ScriptableObject
{
    [SerializeField, HideInInspector] private string saveId;
    public string SaveId => !string.IsNullOrEmpty(saveId) ? saveId
        : throw new System.InvalidOperationException($"Game asset '{name}' needs a save ID.");

#if UNITY_EDITOR
    protected void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += UpdateSaveId;
    }

    private void UpdateSaveId()
    {
        if (this && UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this, out string guid, out long _)
            && saveId != guid)
        {
            saveId = guid;
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
