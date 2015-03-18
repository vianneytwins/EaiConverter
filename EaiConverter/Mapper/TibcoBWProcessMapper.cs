using System;
using System.Collections.Generic;
using TibcoBWConverter.CodeGenerator;
using System.Text;
using TibcoBWConverter.Model;

namespace TibcoBWConverter.Mapper
{
	public class TibcoBWProcessMapper
	{

		public List<ClassModel> Build (TibcoBWProcess tibcoBwProcessToGenerate){

			var tibcoBwProcessClassModel = new ClassModel ();
			tibcoBwProcessClassModel.Name = tibcoBwProcessToGenerate.ProcessName;

			tibcoBwProcessClassModel.Namespace = tibcoBwProcessToGenerate.NameSpace;

			// 2 les using : de chaque namespace d'activite -> toujours les meme car peut pas savoir les domaines avant sauf si veut les classer par Type d'activiter
			tibcoBwProcessClassModel.Imports = this.GenerateImport (tibcoBwProcessToGenerate);

			// 3 les membres privee : les activité injecte
			tibcoBwProcessClassModel.Fields = this.GeneratePrivateFields (tibcoBwProcessToGenerate);

			// 4 le ctor avec injection des activités + logger
			tibcoBwProcessClassModel.Constructors = this.GenerateConstructors (tibcoBwProcessToGenerate, tibcoBwProcessClassModel);

			// 5 les properties : ici y en a pas
			//this.GenerateProperties (tibcoBwProcessToGenerate);

			// 6 la methode start avec input starttype et return du endtype
			tibcoBwProcessClassModel.Methods = this.GenerateMethod (tibcoBwProcessToGenerate);

			var classModels = new List<ClassModel> { tibcoBwProcessClassModel };

			//7 Mappe les classes des activity
//			classModels.AddRange (this.GenerateActivityClasses (tibcoBwProcessToGenerate));
//			if (tibcoBwProcessToGenerate.EndActivity!= null && tibcoBwProcessToGenerate.EndActivity.AdditionnalClassesToGenerate != null) {
//				classModels.AddRange (tibcoBwProcessToGenerate.EndActivity.AdditionnalClassesToGenerate);
//			}
//			if (tibcoBwProcessToGenerate.StartActivity!= null && tibcoBwProcessToGenerate.StartActivity.AdditionnalClassesToGenerate != null) {
//				classModels.AddRange (tibcoBwProcessToGenerate.StartActivity.AdditionnalClassesToGenerate);
//			}
			return classModels;
		}

		public List<string> GenerateImport (TibcoBWProcess tibcoBwProcessToGenerate)
		{
			return new List<string> {
				"System",
				TibcoBWConverterConstant.ActivityNameSpace,
				tibcoBwProcessToGenerate.InputAndOutputNameSpace
			};
		}

		public List<ClassParameter> GeneratePrivateFields (TibcoBWProcess tibcoBwProcessToGenerate)
		{
			var fields = new List<ClassParameter> ();
			fields.Add (new ClassParameter {
				Type = "ILogger",
				Name= "logger",
				IsReadOnly = true
			});
			foreach (Activity activity in tibcoBwProcessToGenerate.Activities) {
				fields.Add (new ClassParameter {
					Type = "I" + VariableHelper.ToClassName (activity.Name + "Service"),
					Name = VariableHelper.ToVariableName (VariableHelper.ToClassName (activity.Name + "Service")),
					IsReadOnly = true
				});
			}

			return fields;
		}

		public List<Method> GenerateConstructors (TibcoBWProcess tibcoBwProcessToGenerate, ClassModel classModel)
		{

			var constructor = new Method ();
			constructor.IsPublic = true;
			constructor.ReturnParameter = new ClassParameter {
				Name = string.Empty,
				Type = classModel.Name
			};

			var inputParameters = new List<ClassParameter> ();
			var sb = new StringBuilder ();
			foreach (ClassParameter field in classModel.Fields) {
				sb.AppendLine ("#this." + field.Name + " = " + field.Name + ";");
				inputParameters.Add (new ClassParameter {
					Name = field.Name,
					Type = field.Type
				});
			}
			constructor.InputParameters = inputParameters;
			constructor.MethodBody = sb.ToString ();

			return new List<Method> { constructor };
		}

		public void GenerateProperties(TibcoBWProcess tibcoBwProcessToGenerate)
		{
		}
			
		public List<Method> GenerateMethod (TibcoBWProcess tibcoBwProcessToGenerate)
		{
			return new List<Method> {GenerateStartMethod (tibcoBwProcessToGenerate)};
		}

		// TODO : refactoriser car ca commence a etre tres moche
		public Method GenerateStartMethod (TibcoBWProcess tibcoBwProcessToGenerate)
		{
			var startMethod = new Method ();

			string returnType;
			string returnName;
			if (tibcoBwProcessToGenerate.EndActivity ==null || tibcoBwProcessToGenerate.EndActivity.Parameters == null) {
				returnType = "void";
				returnName = string.Empty;
			}
			else {
				returnType = tibcoBwProcessToGenerate.EndActivity.Parameters [0].Type;
				returnName = VariableHelper.ToVariableName (tibcoBwProcessToGenerate.EndActivity.Parameters [0].Name);
			}
			startMethod.ReturnParameter = new ClassParameter {
				Name = returnName,
				Type = returnType
			};

			string inputType;
			string inputName;
			if (tibcoBwProcessToGenerate.StartActivity != null){
				startMethod.Name = tibcoBwProcessToGenerate.StartActivity.Name;
			}

			if (tibcoBwProcessToGenerate.StartActivity == null || tibcoBwProcessToGenerate.StartActivity.Parameters == null) {
				inputType = string.Empty;
				inputName = string.Empty;
			}
			else {

				inputType = tibcoBwProcessToGenerate.StartActivity.Parameters [0].Type;
				inputName = tibcoBwProcessToGenerate.StartActivity.Parameters [0].Name;
				startMethod.InputParameters = new List<ClassParameter> {
					new ClassParameter {
						Name = inputName,
						Type = inputType
					}
				};
			}

			var sb = new StringBuilder ();
			if (returnType != "void") {
				sb.AppendLine (@"#var " + startMethod.ReturnParameter.Name + " = new " + returnType + "();");
				sb.AppendLine ("#");
				sb.AppendLine ("#return " + startMethod.ReturnParameter.Name + ";");
			}
			startMethod.MethodBody = sb.ToString();

			return startMethod;

		}

		public List<ClassModel> GenerateActivityClasses (TibcoBWProcess tibcoBwProcessToGenerate)
		{
			//TODO factory
			var jdbcQueryActivityMapper = new JdbcQueryActivityMapper ();
			var activityClasses = new List<ClassModel> ();
			foreach (var activity in tibcoBwProcessToGenerate.Activities) {
				if (activity.Type == JdbcQueryActivity.jdbcQueryActivityType) {
					activityClasses.AddRange (jdbcQueryActivityMapper.Build ((JdbcQueryActivity) activity));
				}
			}
			return activityClasses;
		}
	}
}

