using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CompanyEmployees.ModelBinders
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if(!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            // We extract the value (a comma-separated string of GUIDs) with the
            // ValueProvider.GetValue() expression. Because it is type string, we just
            // check whether it is null or empty.
            var providedValue = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName)
                .ToString();
            if(string.IsNullOrEmpty(providedValue))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            // With the reflection help, we store the type the IEnumerable consists of. In our case, it is GUID.
            var genericType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
            // we create a converter to a GUID type. We didn’t just force the GUID type in this model binder;
            // instead, we inspected what is the nested type of the IEnumerable parameter
            // and then created a converter for that exact type, thus making this binder generic.
            var converter = TypeDescriptor.GetConverter(genericType);

            // We create an array of type object (objectArray) that consist of all
            // the GUID values we sent to the API
            var objectArray = providedValue.Split(new[] { "," },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(x => converter.ConvertFromString(x.Trim()))
                .ToArray();

            // We create an array of GUID types (guidArray),
            // copy all the values from the objectArray to the guidArray,
            // and assign it to the bindingContext
            var guidArray = Array.CreateInstance(genericType, objectArray.Length);
            objectArray.CopyTo(guidArray, 0);
            bindingContext.Model = guidArray;

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
