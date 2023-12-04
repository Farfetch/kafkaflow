using System.Collections.Generic;
using System.Linq;

namespace KafkaFlow.Producers;

internal class ProducerAccessor : IProducerAccessor
{
    private readonly Dictionary<string, IMessageProducer> _producers;

    public ProducerAccessor(IEnumerable<IMessageProducer> producers)
    {
        _producers = producers.ToDictionary(x => x.ProducerName);
    }

    public IEnumerable<IMessageProducer> All => _producers.Values;

    public IMessageProducer this[string name] => this.GetProducer(name);

    public IMessageProducer GetProducer(string name) =>
        _producers.TryGetValue(name, out var consumer) ? consumer : null;

    public IMessageProducer GetProducer<TProducer>() =>
        _producers.TryGetValue(typeof(TProducer).FullName!, out var consumer) ? consumer : null;
}
