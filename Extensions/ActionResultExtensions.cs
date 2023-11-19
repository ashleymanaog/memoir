using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ThomasianMemoir.Extensions
{
    public static class ActionResultExtensions
    {
        public static string RenderToString(this Controller controller, string viewName, object model = null)
        {
            var serviceProvider = controller.HttpContext.RequestServices;
            var viewEngine = serviceProvider.GetRequiredService<ICompositeViewEngine>();

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), controller.ModelState)
            {
                Model = model
            };

            using (var sw = new StringWriter())
            {
                var viewEngineResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                if (!viewEngineResult.Success)
                {
                    throw new InvalidOperationException($"Couldn't find view {viewName}");
                }

                var view = viewEngineResult.View;

                var viewContext = new ViewContext(
                    controller.ControllerContext,
                    view,
                    viewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                view.RenderAsync(viewContext).GetAwaiter().GetResult();

                return sw.ToString();
            }
        }
    }
}
