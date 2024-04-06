using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;




public class PlayerController : NetworkBehaviour
{


    [SerializeField] private Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;

    private NetworkVariable<CustomData> randomNumber = new NetworkVariable<CustomData>(new CustomData
    {
        _int = 100,
        _bool = true,
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);



    public struct CustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }

    }

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (CustomData previousValue, CustomData newValue) =>
        {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
        };
    }



    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Destroy(spawnedObjectTransform.gameObject);
        }

            Vector3 moveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) moveDir.z = + 1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = - 1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z = - 1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = + 1f;

        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }


    [ServerRpc]
    public void TestServerRpc(ServerRpcParams ServerRpcParams)
    {
        Debug.Log("TestServerRpc " + OwnerClientId + "; " + ServerRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    public void TestClientRpc(ClientRpcParams clientRpcParams)
    {
        Debug.Log("TestClientRpc");
    }


}
