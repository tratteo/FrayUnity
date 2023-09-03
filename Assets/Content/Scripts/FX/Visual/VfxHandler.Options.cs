using Fray.Systems.Weapons;
using UnityEngine;

namespace Fray.FX
{
    public partial class VfxHandler
    {
        /// <summary>
        ///   Class specifying how to display a <see cref="VfxHandler"/>
        /// </summary>
        public class Options
        {
            public Shape shape;
            public Attachment attachment;
            public Spatial spatial;
            public Appearance appearance;

            public Options()
            {
                shape = null;
                attachment = new Attachment();
                spatial = new Spatial();
                appearance = new Appearance();
            }

            public static Options ForSwordAttack(SwordAttack attack, Vector3 pos, Vector3 dir)
            {
                return new Options()
                {
                    spatial = new Spatial() { position = pos + attack.Offset, direction = dir, simulationSpace = ParticleSystemSimulationSpace.World, scale = new Vector3(2F, 1F, 1F) * attack.Range },
                    attachment = new Attachment() { parentAttachment = Attachment.ParentAttachment.None },
                };
            }

            public static Options ForSocket(VfxSocket socket)
            {
                return new Options()
                {
                    attachment = new Attachment() { parentAttachment = Attachment.ParentAttachment.Socket, customParent = socket.transform },
                    shape = new Shape()
                    {
                        shapeType = socket.Shape switch
                        {
                            VfxSocket.ShapeType.Circle => ParticleSystemShapeType.Circle,
                            VfxSocket.ShapeType.Rectangle => ParticleSystemShapeType.Rectangle,
                            _ => ParticleSystemShapeType.Rectangle
                        },
                        rectangleScale = socket.Scale,
                        circleRadius = socket.Radius
                    }
                };
            }

            public class Appearance
            {
                public Color? color;

                public Appearance()
                {
                    color = null;
                }
            }

            public class Spatial
            {
                public Quaternion rotation;
                public Vector3 position;
                public Vector3 direction;
                public Vector3 flip;
                public Vector3? scale;
                public ParticleSystemSimulationSpace simulationSpace;
                public ParticleSystemStopAction stopAction;

                public Spatial()
                {
                    rotation = Quaternion.identity;
                    position = Vector3.zero;
                    direction = Vector3.zero;
                    flip = Vector3.zero;
                    simulationSpace = ParticleSystemSimulationSpace.Local;
                    stopAction = ParticleSystemStopAction.None;
                    scale = null;
                }
            }

            public class Attachment
            {
                public enum ParentAttachment
                { Caster, Custom, None, Socket }

                public ParentAttachment parentAttachment;
                public Transform customParent;

                public Attachment()
                {
                    parentAttachment = ParentAttachment.Caster;
                    customParent = null;
                }
            }

            public class Shape
            {
                public ParticleSystemShapeType shapeType;
                public float circleRadius;
                public Vector3 rectangleScale;

                public Shape()
                {
                    shapeType = ParticleSystemShapeType.Circle;
                    circleRadius = 1F;
                    rectangleScale = Vector3.one;
                }
            }
        }
    }
}