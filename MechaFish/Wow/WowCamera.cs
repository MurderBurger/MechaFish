﻿using System;
using System.Drawing;
using MechaFish.Wow.ObjectManager;
using MechaFish.Wow.Patch;
using MechaFish.Wow.Utils;

namespace MechaFish.Wow {
    public class WowCamera : BaseObject {
        private static readonly Lazy<WowCamera> Camera = new Lazy<WowCamera>(() => new WowCamera());

        private WowCamera() : base(0) {
            var cameraStruct = GameManager.GameMemory.Read<uint>(GameManager.BaseAddress + Addresses.Camera.Struct);
            SetPointer(GameManager.GameMemory.Read<uint>(cameraStruct + Addresses.Camera.Offset));
        }

        public static WowCamera ActiveCamera => Camera.Value;

        public Matrix CameraMatrix {
            get {
                var cameraStruct = GameManager.GameMemory.Read<CameraStruct>(Pointer + Addresses.Camera.Matrix);
                return new Matrix(
                    cameraStruct.M11, cameraStruct.M12, cameraStruct.M13, 0,
                    cameraStruct.M21, cameraStruct.M22, cameraStruct.M23, 0,
                    cameraStruct.M31, cameraStruct.M32, cameraStruct.M33, 0,
                    0, 0, 0, 1f);
            }
        }

        public Vector3 Forward => new Vector3(CameraMatrix.M11, CameraMatrix.M12, CameraMatrix.M13);
        public float Fov => GameManager.GameMemory.Read<float>(Pointer + Addresses.Camera.Fov);
        public Vector3 Position => GameManager.GameMemory.Read<Vector3>(Pointer + Addresses.Camera.Origin);

        public Matrix ProjectionMatrix {
            get {
                var wowWindowSize = GameManager.WindowSize;
                var aspectRatio = 0f;

                if ((wowWindowSize.X > 0) && (wowWindowSize.Y > 0)) {
                    aspectRatio = wowWindowSize.X/(float) wowWindowSize.Y;
                }

                return Matrix.PerspectiveFovRh(Fov*0.6f, aspectRatio, 0.2f, 1600f);
            }
        }

        public Matrix ViewMatrix {
            get {
                var position = Position + Forward;
                return Matrix.LookAtRh(Position, position, Vector3.Up);
            }
        }

        public bool WorldToScreen(Vector3 position, ref Point result) {
            var wowWindowSize = GameManager.WindowSize;
            var vector3 = Vector3.Project(position, 0f, 0f, wowWindowSize.X, wowWindowSize.Y, 0f, 1000f,
                ViewMatrix*ProjectionMatrix*Matrix.Identity);

            result.X = (int) vector3.X;
            result.Y = (int) vector3.Y;

            if ((vector3.X <= 0f) || (vector3.X > wowWindowSize.X) || (vector3.Y <= 0f) ||
                (vector3.Y >= wowWindowSize.Y)) {
                return false;
            }

            result.X = (int) vector3.X;
            result.Y = (int) vector3.Y;

            return true;
        }

        private struct CameraStruct {
            public readonly float M11;
            public readonly float M12;
            public readonly float M13;
            public readonly float M21;
            public readonly float M22;
            public readonly float M23;
            public readonly float M31;
            public readonly float M32;
            public readonly float M33;

            // ReSharper disable once UnusedMember.Local
            public CameraStruct(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32,
                float m33) {
                M11 = m11;
                M12 = m12;
                M13 = m13;
                M21 = m21;
                M22 = m22;
                M23 = m23;
                M31 = m31;
                M32 = m32;
                M33 = m33;
            }
        }
    }
}