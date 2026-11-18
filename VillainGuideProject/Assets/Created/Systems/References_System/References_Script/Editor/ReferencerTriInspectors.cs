using TriInspector.Editors;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Main_Referencer))]
public class MainReferencerTriInspector : TriEditor
{
}

[CanEditMultipleObjects]
[CustomEditor(typeof(VisualReferencer))]
public class VisualReferencerTriInspector : TriEditor
{
}

[CanEditMultipleObjects]
[CustomEditor(typeof(ValuesReferencer))]
public class ValuesReferencerTriInspector : TriEditor
{
}

[CanEditMultipleObjects]
[CustomEditor(typeof(MonsterReferencer))]
public class MonsterReferencerTriInspector : TriEditor
{
}
