using Airports;

public class CustomAnnotationEventArgs : EventArgs
{
    public CustomAnnotation Annotation { get; }

    public CustomAnnotationEventArgs(CustomAnnotation annotation)
    {
        Annotation = annotation;
    }
}
