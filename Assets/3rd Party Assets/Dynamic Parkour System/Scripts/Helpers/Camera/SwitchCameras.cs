/*
Dynamic Parkour System grants parkour capabilities to any humanoid character model.
Copyright (C) 2021  Èric Canela Sol
Contact: knela96@gmail.com or @knela96 twitter

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;
using Photon.Pun;

namespace Climbing
{
    public class SwitchCameras : MonoBehaviour
    {
        // Start is called before the first frame update
        Animator animator;

        enum CameraType
        {
            None,
            Freelook,
            Slide
        }

        CameraType curCam = CameraType.None;

        [SerializeField] private CinemachineFreeLook FreeLook;
        [SerializeField] private CinemachineVirtualCamera Slide;


        private IEnumerator Start()
        {
            animator = GetComponent<Animator>();

            yield return new WaitUntil(() => RFM.Globals.player);
            SetCameraTargets();

            FreeLookCam();
        }

        //Switches To FreeLook Cam
        public void FreeLookCam()
        {
            if (curCam != CameraType.Freelook)
            {
                Slide.Priority = 0;
                FreeLook.Priority = 1;
            }
        }

        //Switches To Slide Cam
        public void SlideCam()
        {
            if (curCam != CameraType.Slide)
            {
                FreeLook.Priority = 0;
                Slide.Priority = 1;
            }
        }

        private void LateUpdate()
        {
            if (Time.frameCount % 10 == 0) // to check after every 10 frames
            {
                if (!FreeLook.Follow)
                {
                    Debug.LogError("RFM Reassigning player to RFMCamera");
                    SetCameraTargets();
                }
            }
        }

        private void SetCameraTargets()
        {
            if (!RFM.Globals.player) return;

            var cameraController = RFM.Globals.player.GetComponentInChildren<Climbing.CameraController>();
            var controller = RFM.Globals.player.GetComponentInChildren<Climbing.ThirdPersonController>();

            FreeLook.transform.position = controller.transform.position;
            Slide.transform.position = controller.transform.position;

            FreeLook.Follow = cameraController.playerModel;
            FreeLook.LookAt = cameraController.focus;
            Slide.Follow = cameraController.playerModel;

            controller.mainCamera = transform;
            controller.freeCamera = FreeLook.transform;
            controller.runCamera = FreeLook;
            controller.sliderCamera = Slide;
        }
    }
}
