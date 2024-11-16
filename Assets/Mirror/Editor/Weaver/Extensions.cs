using System;
using System.Collections.Generic;
using System.Linq;
using Mono.CecilX;

namespace Mirror.Weaver
{
    public static class Extensions
    {
        public static bool Is(this TypeReference td, Type t)
        {
            if (t.IsGenericType)
            {
                return td.GetElementType().FullName == t.FullName;
            }
            return td.FullName == t.FullName;
        }

        public static bool Is<T>(this TypeReference td) => Is(td, typeof(T));

        public static bool IsDerivedFrom<T>(this TypeReference tr) => IsDerivedFrom(tr, typeof(T));

        public static bool IsDerivedFrom(this TypeReference tr, Type baseClass)
        {
            TypeDefinition td = tr.Resolve();
            if (!td.IsClass)
                return false;

            // are ANY parent classes of baseClass?
            TypeReference parent = td.BaseType;

            if (parent == null)
                return false;

            if (parent.Is(baseClass))
                return true;

            if (parent.CanBeResolved())
                return IsDerivedFrom(parent.Resolve(), baseClass);

            return false;
        }

        public static TypeReference GetEnumUnderlyingType(this TypeDefinition td)
        {
            foreach (FieldDefinition field in td.Fields)
            {
                if (!field.IsStatic)
                    return field.FieldType;
            }
            throw new ArgumentException($"Invalid enum {td.FullName}");
        }

        public static bool ImplementsInterface<TInterface>(this TypeDefinition td)
        {
            TypeDefinition typedef = td;

            while (typedef != null)
            {
                if (typedef.Interfaces.Any(iface => iface.InterfaceType.Is<TInterface>()))
                    return true;

                try
                {
                    TypeReference parent = typedef.BaseType;
                    typedef = parent?.Resolve();
                }
                catch (AssemblyResolutionException)
                {
                    // this can happen for plugins.
                    //Console.WriteLine("AssemblyResolutionException: "+ ex.ToString());
                    break;
                }
            }

            return false;
        }

        public static bool IsMultidimensionalArray(this TypeReference tr)
        {
            return tr is ArrayType arrayType && arrayType.Rank > 1;
        }

<<<<<<< Updated upstream
        /// <summary>
        /// Does type use netId as backing field
        /// </summary>
        public static bool IsNetworkIdentityField(this TypeReference tr)
        {
            return tr.Is<UnityEngine.GameObject>()
                || tr.Is<NetworkIdentity>()
                || tr.IsDerivedFrom<NetworkBehaviour>();
        }
=======
        // Does type use netId as backing field
        public static bool IsNetworkIdentityField(this TypeReference tr) =>
            tr.Is<UnityEngine.GameObject>() ||
            tr.Is<NetworkIdentity>() ||
            tr.IsDerivedFrom<NetworkBehaviour>();
>>>>>>> Stashed changes

        public static bool CanBeResolved(this TypeReference parent)
        {
            while (parent != null)
            {
                if (parent.Scope.Name == "Windows")
                {
                    return false;
                }

                if (parent.Scope.Name == "mscorlib")
                {
                    TypeDefinition resolved = parent.Resolve();
                    return resolved != null;
                }

                try
                {
                    parent = parent.Resolve().BaseType;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Makes T => Variable and imports function
        /// </summary>
        /// <param name="generic"></param>
        /// <param name="variableReference"></param>
        /// <returns></returns>
        public static MethodReference MakeGeneric(this MethodReference generic, TypeReference variableReference)
        {
            GenericInstanceMethod instance = new GenericInstanceMethod(generic);
            instance.GenericArguments.Add(variableReference);

            MethodReference readFunc = Weaver.CurrentAssembly.MainModule.ImportReference(instance);
            return readFunc;
        }

        /// <summary>
        /// Given a method of a generic class such as ArraySegment`T.get_Count,
        /// and a generic instance such as ArraySegment`int
        /// Creates a reference to the specialized method  ArraySegment`int`.get_Count
        /// <para> Note that calling ArraySegment`T.get_Count directly gives an invalid IL error </para>
        /// </summary>
        /// <param name="self"></param>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public static MethodReference MakeHostInstanceGeneric(this MethodReference self, GenericInstanceType instanceType)
        {
            MethodReference reference = new MethodReference(self.Name, self.ReturnType, instanceType)
            {
                CallingConvention = self.CallingConvention,
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis
            };

            foreach (ParameterDefinition parameter in self.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            foreach (GenericParameter generic_parameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(generic_parameter.Name, reference));

            return Weaver.CurrentAssembly.MainModule.ImportReference(reference);
        }

        /// <summary>
        /// Given a field of a generic class such as Writer<T>.write,
        /// and a generic instance such as ArraySegment`int
        /// Creates a reference to the specialized method  ArraySegment`int`.get_Count
        /// <para> Note that calling ArraySegment`T.get_Count directly gives an invalid IL error </para>
        /// </summary>
        /// <param name="self"></param>
        /// <param name="instanceType">Generic Instance e.g. Writer<int></param>
        /// <returns></returns>
        public static FieldReference SpecializeField(this FieldReference self, GenericInstanceType instanceType)
        {
            FieldReference reference = new FieldReference(self.Name, self.FieldType, instanceType);
            return Weaver.CurrentAssembly.MainModule.ImportReference(reference);
        }

        public static CustomAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider method)
        {
            return method.CustomAttributes.FirstOrDefault(ca => ca.AttributeType.Is<TAttribute>());
        }

        public static bool HasCustomAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider)
        {
            return attributeProvider.CustomAttributes.Any(attr => attr.AttributeType.Is<TAttribute>());
        }

        public static T GetField<T>(this CustomAttribute ca, string field, T defaultValue)
        {
            foreach (CustomAttributeNamedArgument customField in ca.Fields)
                if (customField.Name == field)
                    return (T)customField.Argument.Value;
            return defaultValue;
        }

        public static MethodDefinition GetMethod(this TypeDefinition td, string methodName)
        {
            return td.Methods.FirstOrDefault(method => method.Name == methodName);
        }

        public static List<MethodDefinition> GetMethods(this TypeDefinition td, string methodName)
        {
            return td.Methods.Where(method => method.Name == methodName).ToList();
        }

        public static MethodDefinition GetMethodInBaseType(this TypeDefinition td, string methodName)
        {
            TypeDefinition typedef = td;
            while (typedef != null)
            {
                foreach (MethodDefinition md in typedef.Methods)
                {
                    if (md.Name == methodName)
                        return md;
                }

                try
                {
                    TypeReference parent = typedef.BaseType;
                    typedef = parent?.Resolve();
                }
                catch (AssemblyResolutionException)
                {
                    // this can happen for plugins.
                    break;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds public fields in type and base type
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static IEnumerable<FieldDefinition> FindAllPublicFields(this TypeReference variable)
        {
            return FindAllPublicFields(variable.Resolve());
        }

        /// <summary>
        /// Finds public fields in type and base type
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static IEnumerable<FieldDefinition> FindAllPublicFields(this TypeDefinition typeDefinition)
        {
            while (typeDefinition != null)
            {
                foreach (FieldDefinition field in typeDefinition.Fields)
                {
                    if (field.IsStatic || field.IsPrivate)
                        continue;

                    if (field.IsNotSerialized)
                        continue;

                    yield return field;
                }

                try
                {
                    typeDefinition = typeDefinition.BaseType?.Resolve();
                }
                catch (AssemblyResolutionException)
                {
                    break;
                }
            }
        }
<<<<<<< Updated upstream
=======

        public static bool ContainsClass(this ModuleDefinition module, string nameSpace, string className) =>
            module.GetTypes().Any(td => td.Namespace == nameSpace &&
                                  td.Name == className);


        public static AssemblyNameReference FindReference(this ModuleDefinition module, string referenceName)
        {
            foreach (AssemblyNameReference reference in module.AssemblyReferences)
            {
                if (reference.Name == referenceName)
                    return reference;
            }
            return null;
        }

        // Takes generic arguments from child class and applies them to parent reference, if possible
        // eg makes `Base<T>` in Child<int> : Base<int> have `int` instead of `T`
        // Originally by James-Frowen under MIT 
        // https://github.com/MirageNet/Mirage/commit/cf91e1d54796866d2cf87f8e919bb5c681977e45
        public static TypeReference ApplyGenericParameters(this TypeReference parentReference,
            TypeReference childReference)
        {
            // If the parent is not generic, we got nothing to apply
            if (!parentReference.IsGenericInstance)
                return parentReference;

            GenericInstanceType parentGeneric = (GenericInstanceType)parentReference;
            // make new type so we can replace the args on it
            // resolve it so we have non-generic instance (eg just instance with <T> instead of <int>)
            // if we don't cecil will make it double generic (eg INVALID IL)
            GenericInstanceType generic = new GenericInstanceType(parentReference.Resolve());
            foreach (TypeReference arg in parentGeneric.GenericArguments)
                generic.GenericArguments.Add(arg);

            for (int i = 0; i < generic.GenericArguments.Count; i++)
            {
                // if arg is not generic
                // eg List<int> would be int so not generic.
                // But List<T> would be T so is generic
                if (!generic.GenericArguments[i].IsGenericParameter)
                    continue;

                // get the generic name, eg T
                string name = generic.GenericArguments[i].Name;
                // find what type T is, eg turn it into `int` if `List<int>`
                TypeReference arg = FindMatchingGenericArgument(childReference, name);

                // import just to be safe
                TypeReference imported = parentReference.Module.ImportReference(arg);
                // set arg on generic, parent ref will be Base<int> instead of just Base<T>
                generic.GenericArguments[i] = imported;
            }

            return generic;
        }

        // Finds the type reference for a generic parameter with the provided name in the child reference
        // Originally by James-Frowen under MIT 
        // https://github.com/MirageNet/Mirage/commit/cf91e1d54796866d2cf87f8e919bb5c681977e45
        static TypeReference FindMatchingGenericArgument(TypeReference childReference, string paramName)
        {
            TypeDefinition def = childReference.Resolve();
            // child class must be generic if we are in this part of the code
            // eg Child<T> : Base<T>  <--- child must have generic if Base has T
            // vs Child : Base<int> <--- wont be here if Base has int (we check if T exists before calling this)
            if (!def.HasGenericParameters)
                throw new InvalidOperationException(
                    "Base class had generic parameters, but could not find them in child class");

            // go through parameters in child class, and find the generic that matches the name
            for (int i = 0; i < def.GenericParameters.Count; i++)
            {
                GenericParameter param = def.GenericParameters[i];
                if (param.Name == paramName)
                {
                    GenericInstanceType generic = (GenericInstanceType)childReference;
                    // return generic arg with same index
                    return generic.GenericArguments[i];
                }
            }

            // this should never happen, if it does it means that this code is bugged
            throw new InvalidOperationException("Did not find matching generic");
        }
>>>>>>> Stashed changes
    }
}
