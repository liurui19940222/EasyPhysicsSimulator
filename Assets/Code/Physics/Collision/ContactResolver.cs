using System.Collections.Generic;

namespace Physics {

    public class ContactResolver {
    
        public void ResolveContacts(List<Contact> contacts, int iterations, float deltaTime) {
            //int _iterationCount = 0;
            //float min, sepVel;
            //int minIndex;
            //while (_iterationCount < iterations) {
            //    min = 0;
            //    minIndex = 0;
            //    for (int i = 0; i < contacts.Count; ++i) {
            //        sepVel = contacts[i].CalculateSeparatingVelocity();
            //        if (sepVel < min) {
            //            min = sepVel;
            //            minIndex = i;
            //        }
            //    }
            //    contacts[minIndex].Resolve(deltaTime);
            //    _iterationCount++;
            //}
            for (int i = 0; i < contacts.Count; ++i) {
                contacts[i].Resolve(deltaTime);
            }
        }

    }

}
