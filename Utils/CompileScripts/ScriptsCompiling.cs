using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;

namespace Utils.CompileScripts
{
    /// <summary>
    /// Implements scripts compiling
    /// </summary>
    public static class ScriptsCompiling
    {
        /// <summary>
        /// Compiles assembly from scenarios
        /// </summary>
        /// <param name="scenarioItems">scenario list</param>
        /// <param name="file">file for compiling</param>
        /// <returns></returns>
        public static Assembly CompileAssembly(IList<ScenarioItem> scenarioItems, string file)
        {
            using (CodeDomProvider comp = new CSharpCodeProvider())
            {
                CompilerParameters cp = new CompilerParameters();
                cp.ReferencedAssemblies.Add("system.dll");
                cp.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
                cp.GenerateExecutable = false;
                cp.GenerateInMemory = true;

                //Create class derived from ScriptingBase
                StringBuilder code = new StringBuilder();
                code.Append("using System; \n");
                code.Append("namespace Utils.CompileScripts { \n");
                code.Append("  public class _Executor : ScriptingBase { \n");
                foreach (ScenarioItem item in scenarioItems)
                {
                    code.AppendFormat("    public ScenarioDelegate GetScenarioDelegate{0}() ",
                                      item.id);
                    code.Append("{ return delegate() { ");
                    code.Append(item.scriptingExpression);
                    code.Append(";};}\n");
                }
                code.Append("} }");

                CompilerResults cr;
                //Compiling class into assembly
                if (string.IsNullOrEmpty(file))
                {
                    cr = comp.CompileAssemblyFromSource(cp, code.ToString());
                }
                else
                {
                    File.WriteAllText(file, code.ToString());
                    cr = comp.CompileAssemblyFromFile(cp, file);
                }

                if (cr.Errors.HasErrors)
                {
                    StringBuilder error = new StringBuilder();
                    foreach (CompilerError err in cr.Errors)
                    {
                        if (!string.IsNullOrEmpty(file))
                        {
                            error.AppendFormat("{0}({1},{2}): {3}\n", Path.GetFileName(file), err.Line, err.Column, err.ErrorText);
                        }
                        else
                        {
                            error.AppendFormat("Error: {0} \n", err.ErrorText);
                        }
                    }
                    throw new CompileErrorException("Error Compiling Expression: \n" + error, file);
                }
                return cr.CompiledAssembly;
            }
        }
    }
}
