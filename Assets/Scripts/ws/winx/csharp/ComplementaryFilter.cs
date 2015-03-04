using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ws.winx.csharp
{

    public class ComplementaryFilter
    {



        // Marks first sample.
        bool firstSample;



        /// <summary>
        ///  Angles X,Y,Z in radians
        /// </summary>
        Vector3 _Angles = new Vector3();

        Vector3 _AnglesIntegrated = new Vector3();

        // Gyro weight/smooting factor.
        public float w1;
        public float w2;


        /// <summary>
        /// Angles in rad
        /// </summary>
        public Vector3 Angles
        {
            get { return _Angles; }

        }




        public ComplementaryFilter(float weight1 = 0.98f, float weight2 = 0.02f)
        {
            w1 = weight1;
            w2 = weight2;
            firstSample = true;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="accX"></param>
        /// <param name="accY"></param>
        /// <param name="accZ"></param>
        /// <param name="gx">Gyro X angle rate in rad/s </param>
        /// <param name="gy"></param>
        /// <param name="gz"></param>
        /// <param name="dt">seconds</param>
        public void Update(double accX, double accY, double accZ, double gx, double gy, double gz,   double dt)
        {

          

            // Integrate the gyroscope data -> int(angularSpeed) = angle
            //*pitch += ((float)gyrData[0] / GYROSCOPE_SENSITIVITY) * dt; // Angle around the X-axis
            //*roll -= ((float)gyrData[1] / GYROSCOPE_SENSITIVITY) * dt;    // Angle around the Y-axis

            double xSquared = accX * accX;
            double ySquared = accY * accY;
            double zSquared = accZ * accZ;


            double magnitude = Math.Sqrt(xSquared + ySquared + zSquared);

            if (magnitude == 0) return;

            double inv_len = 1 / magnitude;
            double x = accX * inv_len;
            double y = accY * inv_len;
            double z = accZ * inv_len;

            if (firstSample)
            {
                _AnglesIntegrated.y = (float)Math.Atan2(x, z);
                _AnglesIntegrated.x = -(float)Math.Atan2(y, z);// -(float)Math.Asin(y);

                _Angles.x += (float)Math.Atan2(x, z);
                _Angles.y += -(float)Math.Atan2(y, z); //-(float)Math.Asin(y);




                firstSample = false;
            }
            else
            {
                _AnglesIntegrated.x += (float)(gx * dt);
                //   _AnglesIntegrated.x /= (float)(2 * Math.PI);

                if (_AnglesIntegrated.x > Math.PI)
                    _AnglesIntegrated.x += -(float)(2 * Math.PI);
                else if (_AnglesIntegrated.x < -Math.PI)
                    _AnglesIntegrated.x += (float)(2 * Math.PI);


                _AnglesIntegrated.y += (float)(gy * dt);


                if (_AnglesIntegrated.y > Math.PI)
                    _AnglesIntegrated.y += -(float)(2 * Math.PI);
                else if (_AnglesIntegrated.y < -Math.PI)
                    _AnglesIntegrated.y += (float)(2 * Math.PI);

           

                //Roll
                _Angles.y = _AnglesIntegrated.y * w1 + w2 * (float)Math.Atan2(x, z);

                //Pitch
                _Angles.x = _AnglesIntegrated.x * w1 - w2 * (float)Math.Atan2(y, z);
            }


            if (Double.IsNaN(_Angles.x))
                _Angles.x = 0f;
            if (Double.IsNaN(_Angles.y))
                _Angles.y = 0f;


            _Angles.z += (float)(gz * dt);
           
        }


    }
}
