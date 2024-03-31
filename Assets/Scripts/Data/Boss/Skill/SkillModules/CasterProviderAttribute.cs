using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DreamLU
{
#if UNITY_2018_4_OR_NEWER
    [JetBrains.Annotations.MeansImplicitUse(
        JetBrains.Annotations.ImplicitUseKindFlags.Access |
        JetBrains.Annotations.ImplicitUseKindFlags.Assign |
        JetBrains.Annotations.ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
#endif
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field)]
    public class CasterProviderAttribute : Attribute
    {
        public readonly bool IsRequire;

        public CasterProviderAttribute(bool isRequire = true)
        {
            IsRequire = isRequire;
        }
    }

    public static class CasterProviderExtension
    {
        private struct FieldWithAttribute<TAttr> where TAttr : Attribute
        {
            public FieldInfo info;
            public TAttr attr;
        }

        private static readonly Dictionary<Type, List<FieldWithAttribute<CasterProviderAttribute>>> _fields = new ();
        
        public static bool InjectCasterProviderFrom(this object target, ICaster source)
        {
            var targetType = target.GetType();
            var fields = GetCasterProviderFields(targetType);
            
            foreach (var field in fields)
            {
                // auto set field value if caster implemented this interface
                if (field.info.FieldType.IsInstanceOfType(source))
                {
                    field.info.SetValue(target, source); 
                } 
                else // try find external provider from caster
                {
                    var provider = source.GetExternalProvider(field.info.FieldType);
                    if (provider != null && field.info.FieldType.IsInstanceOfType(provider))
                        field.info.SetValue(target, provider);
                    else if (field.attr.IsRequire) // error: Couldn't find requirement interface
                    {
                        Debug.LogError($"{target.GetType().Name} require provider {field.info.FieldType.Name} from {source.GetType().Name}");
                        return false;
                    }
                }
            }

            return true;
        }

        private static List<FieldWithAttribute<CasterProviderAttribute>> GetCasterProviderFields(Type type)
        {
            if (_fields.TryGetValue(type, out var fields))
                return fields;

            fields = new List<FieldWithAttribute<CasterProviderAttribute>>();
            var allFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in allFields)
            {
                if (!field.FieldType.IsInterface) continue;
                
                var attr = field.GetCustomAttribute<CasterProviderAttribute>();
                if (attr == null) continue;

                fields.Add(new FieldWithAttribute<CasterProviderAttribute>
                {
                    info = field,
                    attr = attr
                });
            }
            _fields.Add(type, fields);
            return fields;
        }
    }
}
