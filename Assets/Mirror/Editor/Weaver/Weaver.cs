using System;
using System.Collections.Generic;
using System.IO;
using Mono.CecilX;

namespace Mirror.Weaver
{
    // This data is flushed each time - if we are run multiple times in the same process/domain
    class WeaverLists
    {
<<<<<<< Updated upstream
        // setter functions that replace [SyncVar] member variable references. dict<field, replacement>
        public Dictionary<FieldDefinition, MethodDefinition> replacementSetterProperties = new Dictionary<FieldDefinition, MethodDefinition>();
        // getter functions that replace [SyncVar] member variable references. dict<field, replacement>
        public Dictionary<FieldDefinition, MethodDefinition> replacementGetterProperties = new Dictionary<FieldDefinition, MethodDefinition>();
=======
        public const string InvokeRpcPrefix = "InvokeUserCode_";

        // generated code class
        public const string GeneratedCodeNamespace = "Mirror";
        public const string GeneratedCodeClassName = "GeneratedNetworkCode";
        TypeDefinition GeneratedCodeClass;
>>>>>>> Stashed changes

        public TypeDefinition generateContainerClass;

        // amount of SyncVars per class. dict<className, amount>
        public Dictionary<string, int> numSyncVars = new Dictionary<string, int>();

        public int GetSyncVarStart(string className)
        {
            return numSyncVars.ContainsKey(className)
                   ? numSyncVars[className]
                   : 0;
        }

        public void SetNumSyncVars(string className, int num)
        {
            numSyncVars[className] = num;
        }

        public WeaverLists()
        {
            generateContainerClass = new TypeDefinition("Mirror", "GeneratedNetworkCode",
                    TypeAttributes.BeforeFieldInit | TypeAttributes.Class | TypeAttributes.AnsiClass | TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.Abstract | TypeAttributes.Sealed,
                    WeaverTypes.Import<object>());
        }
    }

    internal static class Weaver
    {
        public static string InvokeRpcPrefix => "InvokeUserCode_";

        public static WeaverLists WeaveLists { get; private set; }
        public static AssemblyDefinition CurrentAssembly { get; private set; }
        public static bool WeavingFailed { get; private set; }
        public static bool GenerateLogErrors;

        // private properties
        static readonly bool DebugLogEnabled = true;

        public static void DLog(TypeDefinition td, string fmt, params object[] args)
        {
            if (!DebugLogEnabled)
                return;

            Console.WriteLine("[" + td.Name + "] " + string.Format(fmt, args));
        }

        // display weaver error
        // and mark process as failed
        public static void Error(string message)
        {
            Log.Error(message);
            WeavingFailed = true;
        }

        public static void Error(string message, MemberReference mr)
        {
            Log.Error($"{message} (at {mr})");
            WeavingFailed = true;
        }

        public static void Warning(string message, MemberReference mr)
        {
            Log.Warning($"{message} (at {mr})");
        }


        static void CheckMonoBehaviour(TypeDefinition td)
        {
            if (td.IsDerivedFrom<UnityEngine.MonoBehaviour>())
            {
                MonoBehaviourProcessor.Process(td);
            }
        }

        static bool WeaveNetworkBehavior(TypeDefinition td)
        {
            if (!td.IsClass)
                return false;

            if (!td.IsDerivedFrom<NetworkBehaviour>())
            {
                CheckMonoBehaviour(td);
                return false;
            }

            // process this and base classes from parent to child order

            List<TypeDefinition> behaviourClasses = new List<TypeDefinition>();

            TypeDefinition parent = td;
            while (parent != null)
            {
                if (parent.Is<NetworkBehaviour>())
                {
                    break;
                }

                try
                {
                    behaviourClasses.Insert(0, parent);
                    parent = parent.BaseType.Resolve();
                }
                catch (AssemblyResolutionException)
                {
                    // this can happen for plugins.
                    //Console.WriteLine("AssemblyResolutionException: "+ ex.ToString());
                    break;
                }
            }

            bool modified = false;
            foreach (TypeDefinition behaviour in behaviourClasses)
            {
                modified |= new NetworkBehaviourProcessor(behaviour).Process();
            }
            return modified;
        }

        static bool WeaveModule(ModuleDefinition moduleDefinition)
        {
<<<<<<< Updated upstream
=======
            bool modified = false;

            Stopwatch watch = Stopwatch.StartNew();
            watch.Start();

            foreach (TypeDefinition td in moduleDefinition.Types)
            {
                if (td.IsClass && td.BaseType.CanBeResolved())
                {
                    modified |= WeaveNetworkBehavior(td);
                    modified |= ServerClientAttributeProcessor.Process(weaverTypes, Log, td, ref WeavingFailed);
                }
            }

            watch.Stop();
            Console.WriteLine($"Weave behaviours and messages took {watch.ElapsedMilliseconds} milliseconds");

            return modified;
        }

        void CreateGeneratedCodeClass()
        {
            // create "Mirror.GeneratedNetworkCode" class which holds all
            // Readers<T> and Writers<T>
            GeneratedCodeClass = new TypeDefinition(GeneratedCodeNamespace, GeneratedCodeClassName,
                TypeAttributes.BeforeFieldInit | TypeAttributes.Class | TypeAttributes.AnsiClass | TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.Abstract | TypeAttributes.Sealed,
                weaverTypes.Import<object>());
        }

        // Weave takes an AssemblyDefinition to be compatible with both old and
        // new weavers:
        // * old takes a filepath, new takes a in-memory byte[]
        // * old uses DefaultAssemblyResolver with added dependencies paths,
        //   new uses ...?
        //
        // => assembly: the one we are currently weaving (MyGame.dll)
        // => resolver: useful in case we need to resolve any of the assembly's
        //              assembly.MainModule.AssemblyReferences.
        //              -> we can resolve ANY of them given that the resolver
        //                 works properly (need custom one for ILPostProcessor)
        //              -> IMPORTANT: .Resolve() takes an AssemblyNameReference.
        //                 those from assembly.MainModule.AssemblyReferences are
        //                 guaranteed to be resolve-able.
        //                 Parsing from a string for Library/.../Mirror.dll
        //                 would not be guaranteed to be resolve-able because
        //                 for ILPostProcessor we can't assume where Mirror.dll
        //                 is etc.
        public bool Weave(AssemblyDefinition assembly, IAssemblyResolver resolver, out bool modified)
        {
            WeavingFailed = false;
            modified = false;
>>>>>>> Stashed changes
            try
            {
                bool modified = false;

                System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

                watch.Start();
                foreach (TypeDefinition td in moduleDefinition.Types)
                {
                    if (td.IsClass && td.BaseType.CanBeResolved())
                    {
                        modified |= WeaveNetworkBehavior(td);
                        modified |= ServerClientAttributeProcessor.Process(td);
                    }
                }
                watch.Stop();
                Console.WriteLine("Weave behaviours and messages took" + watch.ElapsedMilliseconds + " milliseconds");

                return modified;
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
                throw new Exception(ex.Message, ex);
            }
        }

        static bool Weave(string assName, IEnumerable<string> dependencies)
        {
            using (DefaultAssemblyResolver asmResolver = new DefaultAssemblyResolver())
            using (CurrentAssembly = AssemblyDefinition.ReadAssembly(assName, new ReaderParameters { ReadWrite = true, ReadSymbols = true, AssemblyResolver = asmResolver }))
            {
                asmResolver.AddSearchDirectory(Path.GetDirectoryName(assName));
                asmResolver.AddSearchDirectory(Helpers.UnityEngineDllDirectoryName());
                if (dependencies != null)
                {
                    foreach (string path in dependencies)
                    {
                        asmResolver.AddSearchDirectory(path);
                    }
                }

                WeaverTypes.SetupTargetTypes(CurrentAssembly);
                // WeaverList depends on WeaverTypes setup because it uses Import
                WeaveLists = new WeaverLists();


                System.Diagnostics.Stopwatch rwstopwatch = System.Diagnostics.Stopwatch.StartNew();
                // Need to track modified from ReaderWriterProcessor too because it could find custom read/write functions or create functions for NetworkMessages
                bool modified = ReaderWriterProcessor.Process(CurrentAssembly);
                rwstopwatch.Stop();
                Console.WriteLine($"Find all reader and writers took {rwstopwatch.ElapsedMilliseconds} milliseconds");

                ModuleDefinition moduleDefinition = CurrentAssembly.MainModule;
                Console.WriteLine($"Script Module: {moduleDefinition.Name}");

                modified |= WeaveModule(moduleDefinition);

                if (WeavingFailed)
                {
                    return false;
                }

                if (modified)
                {
<<<<<<< Updated upstream
                    PropertySiteProcessor.Process(moduleDefinition);
=======
                    SyncVarAttributeAccessReplacer.Process(moduleDefinition, syncVarAccessLists);
>>>>>>> Stashed changes

                    // add class that holds read/write functions
                    moduleDefinition.Types.Add(WeaveLists.generateContainerClass);

                    ReaderWriterProcessor.InitializeReaderAndWriters(CurrentAssembly);

                    // write to outputDir if specified, otherwise perform in-place write
                    WriterParameters writeParams = new WriterParameters { WriteSymbols = true };
                    CurrentAssembly.Write(writeParams);
                }
            }

<<<<<<< Updated upstream
            return true;
        }

        public static bool WeaveAssembly(string assembly, IEnumerable<string> dependencies)
        {
            WeavingFailed = false;

            try
            {
                return Weave(assembly, dependencies);
=======
                return true;
>>>>>>> Stashed changes
            }
            catch (Exception e)
            {
                Log.Error("Exception :" + e);
                return false;
            }
        }

    }
}
