using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;

// Tell Unity that this drawer handles any class marked with ConditionalDrawerAttribute
[CustomPropertyDrawer(typeof(ConditionalDrawerAttribute))]
public class ConditionalDrawer : PropertyDrawer
{
    // Caches the default property height (for 'delay', 'entityType', etc.)
    private float defaultHeight;

    // Caches the property height for fields that are conditionally hidden
    private float hiddenHeight = 0f;

    // --- 1. Calculate the Total Height of the Property ---
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Get the default height from Unity's standard drawing
        defaultHeight = base.GetPropertyHeight(property, label);

        // Reset height and check visibility of all fields
        float totalHeight = 0f;
        hiddenHeight = 0f;

        // Find the 'entityType' property to check the condition
        SerializedProperty entityTypeProp = property.FindPropertyRelative("entityType");

        // Iterate through all child properties (fields) of WaveEvent
        foreach (SerializedProperty childProperty in GetChildProperties(property))
        {
            float childHeight = EditorGUI.GetPropertyHeight(childProperty, true);

            // Check if the current child property should be visible
            if (IsVisible(childProperty, entityTypeProp))
            {
                totalHeight += childHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                // Accumulate the height of properties that are being hidden
                hiddenHeight += childHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        // Return the total height of all visible properties
        // We add back the height of the container label/foldout
        return totalHeight + defaultHeight;
    }

    // --- 2. Draw the Property in the Inspector ---
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Start drawing as a container (the WaveEvent foldout)
        EditorGUI.BeginProperty(position, label, property);

        // Use the default height for the first line (the foldout/label)
        Rect currentRect = new Rect(position.x, position.y, position.width, defaultHeight);

        // Draw the main label/foldout using Unity's default drawing
        EditorGUI.PropertyField(currentRect, property, label, false);

        // Only draw the inner content if the foldout is open
        if (property.isExpanded)
        {
            // Move down past the label line
            currentRect.y += defaultHeight + EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty entityTypeProp = property.FindPropertyRelative("entityType");

            // Iterate through all child properties to draw them
            foreach (SerializedProperty childProperty in GetChildProperties(property))
            {
                float childHeight = EditorGUI.GetPropertyHeight(childProperty, true);
                currentRect.height = childHeight;

                // Check visibility again before drawing
                if (IsVisible(childProperty, entityTypeProp))
                {
                    // Draw the visible property
                    EditorGUI.PropertyField(currentRect, childProperty, true);

                    // Move down for the next property
                    currentRect.y += childHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }
        }

        EditorGUI.EndProperty();
    }

    // --- 3. Custom Logic to Determine Visibility ---
    private bool IsVisible(SerializedProperty property, SerializedProperty entityTypeProp)
    {
        // 1. Always show 'delay' and 'entityType' (the control variables)
        if (property.name == "delay" || property.name == "entityType")
        {
            return true;
        }

        // Get the current enum value as an integer
        int currentEntityType = entityTypeProp.enumValueIndex;

        // Use property.name to check which fields should be visible for the current entityType

        // --- WalkingEnemy (Enum Index 0) ---
        if (currentEntityType == 0) // Entity.WalkingEnemy
        {
            return property.name == "speedBoost" ||
                   property.name == "spawnCount" ||
                   property.name == "spawnPosition" ||
                   property.name == "walkingSpeed";
        }

        // --- FlyingEnemy (Enum Index 1) ---
        else if (currentEntityType == 1) // Entity.FlyingEnemy
        {
            return property.name == "speedBoost" ||
                   property.name == "spawnCount" ||
                   property.name == "flyingHeight";
        }

        // --- DashingEnemy (Enum Index 2) ---
        else if (currentEntityType == 2) // Entity.DashingEnemy
        {
            return property.name == "speedBoost" ||
                   property.name == "spawnCount" ||
                   property.name == "dashDistance";
        }

        // --- DroppedEnemy (Enum Index 3) ---
        else if (currentEntityType == 3) // Entity.DroppedEnemy
        {
            return property.name == "speedBoost" ||
                   property.name == "spawnCount" ||
                   property.name == "dropHeight";
        }

        // --- Boxes (Indices 4, 5, 6) ---
        else if (currentEntityType >= 4 && currentEntityType <= 6) // FlyingBox, GroundBoxBig, GroundBoxSmall
        {
            return property.name == "boxHealth";
        }

        // --- ConveyorBelt (Enum Index 7) ---
        else if (currentEntityType == 7) // Entity.ConveyorBelt
        {
            return property.name == "conveyorSpeed";
        }

        // Hide all other properties if they are not specifically handled
        return false;
    }

    // --- Helper function to get all child properties ---
    private static PropertyInfo GetChildPropertiesInfo(SerializedProperty property)
    {
        return property.GetType().GetProperty("children", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    private static System.Collections.Generic.IEnumerable<SerializedProperty> GetChildProperties(SerializedProperty property)
    {
        // Use the PropertyInfo from reflection (necessary for correct iteration)
        PropertyInfo childrenInfo = GetChildPropertiesInfo(property);
        if (childrenInfo != null)
        {
            return (System.Collections.Generic.IEnumerable<SerializedProperty>)childrenInfo.GetValue(property);
        }

        // Fallback for older Unity versions or if reflection fails
        Debug.LogWarning("Could not retrieve SerializedProperty children using reflection. Falling back to nextVisible option.");
        return FallbackGetChildProperties(property);
    }

    // Fallback iteration method
    private static System.Collections.Generic.IEnumerable<SerializedProperty> FallbackGetChildProperties(SerializedProperty property)
    {
        SerializedProperty currentProperty = property.Copy();
        SerializedProperty nextProperty = property.Copy();

        // Enter the container property
        nextProperty.NextVisible(true);

        // While we are still inside the container property
        while (currentProperty.NextVisible(false) && !SerializedProperty.EqualContents(currentProperty, nextProperty))
        {
            yield return currentProperty;
        }
    }
}