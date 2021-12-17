namespace KafkaFlow.Client.Protocol.Authentication.SASL.Scram
{
    public interface IScramHashMethods
    {
        byte[] Hi(string password, string salt, int count);

        byte[] HMAC(byte[] key, string data);

        byte[] H(byte[] data);
    }
}
