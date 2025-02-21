using k8s;
using k8s.Models;

#nullable enable
namespace HealthChecks.UI.Core.Discovery.K8S.Extensions;

internal static class KubernetesHttpClientExtensions
{
    internal static async Task<V1ServiceList> GetServicesAsync(this IKubernetes client, string label, List<string> k8sNamespaces, CancellationToken cancellationToken)
    {
        if (k8sNamespaces is null || k8sNamespaces.Count == 0)
        {
            return await client.CoreV1.ListServiceForAllNamespacesAsync(labelSelector: label, cancellationToken: cancellationToken);
        }
        else
        {
            var responses = await Task.WhenAll(k8sNamespaces.Select(k8sNamespace => client.CoreV1.ListNamespacedServiceAsync(k8sNamespace, labelSelector: label, cancellationToken: cancellationToken)));

            return new V1ServiceList()
            {
                Items = responses.SelectMany(r => r.Items).ToList()
            };
        }
    }
}
