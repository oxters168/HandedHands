using UnityEngine;
using UnityHelpers;

/// <summary>
/// This component simulates friction being applied from the object it is attached to to any object it comes in contact with. Both objects require rigidbodies for this to work.
/// </summary>
public class SimulateFriction : MonoBehaviour
{
    private Rigidbody affectingBody;
    public float affectingMass = 5;
    public float multiplier = 0.5f;
    public float maxDotValue = 0.5f;

    private void Start()
    {
        affectingBody = GetComponentInParent<Rigidbody>();
        Debug.Assert(affectingBody != null, "SimulateFriction(" + gameObject.name + "): Could not find rigidbody on object or any parent");
    }
    private void OnCollisionStay(Collision collision)
    {
        if (affectingBody != null)
        {
            var affectedBody = collision.gameObject.GetComponentInParent<Rigidbody>();
            if (affectedBody != null)
            {
                var collisionContact = collision.GetContact(0);
                Vector3 addedForce = Mathf.Min(affectingMass, affectedBody.mass) * (affectingBody.velocity / Time.fixedDeltaTime);
                Vector3 addedForceDirection = addedForce.normalized;
                //Vector3 contactNormal = (affectingBody.position - collisionContact.point).normalized;
                Vector3 contactNormal = collisionContact.normal;
                float directionPercent = 1 - (Mathf.Clamp(Mathf.Abs(Vector3.Dot(addedForceDirection, contactNormal)), 0, maxDotValue) / maxDotValue);
                addedForce = Vector3.ProjectOnPlane(addedForce, contactNormal);
                //float massPercent = Mathf.Pow(Mathf.Clamp01(affectingBody.mass / affectedBody.mass), 5);

                //Vector3 subtractedForce = affectedBody.mass * (affectedBody.velocity / Time.fixedDeltaTime);
                //Vector3 subtracedForceDirection = subtractedForce.normalized;
                //float subtractedForcePercent = 1 - (Mathf.Clamp(Mathf.Abs(Vector3.Dot(subtracedForceDirection, collisionContact.normal)), 0, maxDotValue) / maxDotValue);

                affectedBody.AddForce(addedForce * directionPercent * multiplier, ForceMode.Force);

                PoolManager.GetPool("AxisPool").Get((transform) => { transform.localScale = Vector3.one * 0.01f; transform.position = collisionContact.point; transform.forward = contactNormal; });

                //float percentVelocity = Mathf.Clamp01(affectingBody.mass / affectedBody.mass);
                //affectedBody.AddForce(affectingBody.velocity * percentVelocity, ForceMode.VelocityChange);
            }
            else
                Debug.LogWarning("SimulateFriction(" + gameObject.name + "): Could not find rigidbody on " + collision.gameObject.name + " or any of it's parents");
        }
        else
            Debug.LogError("SimulateFriction(" + gameObject.name + "): Cannot apply friction without rigidbody on applier");
    }
}
