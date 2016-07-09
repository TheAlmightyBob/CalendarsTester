using System;
using Xamarin.Forms;

namespace CalendarsTester.Extensions
{
    public static class VisualElementExtensions
    {
        public static Page GetPage(this VisualElement element)
        {
            return element as Page ?? (element.Parent as VisualElement)?.GetPage();
        }

        public static Point TranslatePointToAncestor(this VisualElement element, Point point, VisualElement relativeTo)
        {
            if (relativeTo == null)
            {
                relativeTo = element.GetPage();
            }

            VisualElement parentView;

            if (element == relativeTo)
            {
                return point;
            }
            else if ((parentView = element.Parent as VisualElement) != null)
            {
                return parentView.TranslatePointToAncestor(new Point(point.X + element.X, point.Y + element.Y), relativeTo);
            }
            else
            {
                throw new ArgumentException("Relative element must be an ancestor of original element", "relativeTo");
            }
        }

        public static Point TranslatePointToDescendent(this VisualElement element, Point point, VisualElement relativeTo)
        {
            if (relativeTo == null)
            {
                throw new ArgumentException("Relative element must be a child of original element", "relativeTo");
            }

            if (element == relativeTo)
            {
                return point;
            }
            else
            {
                return element.TranslatePointToDescendent(new Point(point.X - relativeTo.X, point.Y - relativeTo.Y), relativeTo.Parent as VisualElement);
            }
        }
    }
}