import pyrealsense2 as rs
import mediapipe as mp
import cv2
import numpy as np
import datetime as dt
import socket


def count_fingers(landmarks, i, hand_side):
    # Check if hand landmarks are detected
    if landmarks:
        if landmarks[i]:
            hand_landmarks = landmarks[i]
            # Extract landmark positions
            landmarks = []
            for point in hand_landmarks.landmark:
                landmarks.append((point.x, point.y, point.z))

            # Count fingers based on landmark positions
            finger_count = 0

            # Thumb (check if x-coordinate of thumb tip is to the left of the x-coordinate of the thumb IP)
            if hand_side == "right":
                if landmarks[4][0] < landmarks[3][0]:
                    finger_count += 1
            elif hand_side == "left":
                if landmarks[4][0] > landmarks[3][0]:
                    finger_count += 1

            # Index finger
            if landmarks[8][1] < landmarks[6][1]:
                finger_count += 1

            # Middle finger
            if landmarks[12][1] < landmarks[10][1]:
                finger_count += 1

            # Ring finger
            if landmarks[16][1] < landmarks[14][1]:
                finger_count += 1

            # Little finger
            if landmarks[20][1] < landmarks[18][1]:
                finger_count += 1

            return finger_count

    else:
        return 0  # No hand detected


def main():
    host, port = "127.0.0.1", 25001
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    print("waiting for connection to be made")
    # desired_buffer_size = 1000000
    # sock.setsockopt(socket.SOL_SOCKET, socket.SO_SNDBUF, desired_buffer_size)
    # send_buffer_size = sock.getsockopt(socket.SOL_SOCKET, socket.SO_SNDBUF)
    # print(f"Send Buffer Size: {send_buffer_size}")
    try:
        sock.connect((host, port))
    except Exception as e:
        print("start printing")
        print(e)
        print("end printing")
    # receivedData = sock.recv(1024).decode("UTF-8")  # receiveing data in Byte fron C#, and converting it to String
    print("connection been made")
    direction = [0, 0]  # Vector2   x = 0, y = 0

    ##et up the formatting for our OpenCV window that displays the output of our Hand Detection and Tracking
    font = cv2.FONT_HERSHEY_SIMPLEX
    org = (20, 100)
    fontScale = 0.8
    color = (255, 0, 0)
    thickness = 1

    # ====== Realsense ======
    realsense_ctx = rs.context()
    connected_devices = []  # List of serial numbers for present cameras
    for i in range(len(realsense_ctx.devices)):
        detected_camera = realsense_ctx.devices[i].get_info(rs.camera_info.serial_number)
        print(f"{detected_camera}")
        connected_devices.append(detected_camera)
    device = connected_devices[0]  # In this example we are only using one camera
    pipeline = rs.pipeline()
    config = rs.config()
    background_removed_color = 153  # Grey

    # ====== Mediapipe ======
    mpHands = mp.solutions.hands
    hands = mpHands.Hands()
    mpDraw = mp.solutions.drawing_utils

    # ====== Enable Streams ======
    config.enable_device(device)
    # # For worse FPS, but better resolution:
    # stream_res_x = 1280
    # stream_res_y = 720
    # # For better FPS. but worse resolution:
    stream_res_x = 640
    stream_res_y = 480
    stream_fps = 30
    config.enable_stream(rs.stream.depth, stream_res_x, stream_res_y, rs.format.z16, stream_fps)
    config.enable_stream(rs.stream.color, stream_res_x, stream_res_y, rs.format.bgr8, stream_fps)
    profile = pipeline.start(config)
    align_to = rs.stream.color
    align = rs.align(align_to)

    # ====== Get depth Scale ======
    depth_sensor = profile.get_device().first_depth_sensor()
    depth_scale = depth_sensor.get_depth_scale()
    print(f"\tDepth Scale for Camera SN {device} is: {depth_scale}")
    # ====== Set clipping distance ======
    clipping_distance_in_meters = 2
    clipping_distance = clipping_distance_in_meters / depth_scale
    print(f"\tConfiguration Successful for SN {device}")

    finger_count_right, finger_count_left = -1, -1
    is_right_hand_closed, is_left_hand_closed = 0, 0

    right_hand_depth = 0
    left_hand_depth = 0
    hand_diff = 0
    bow_power = 0
    y_left_angel = 0
    y_right_angel = 0
    left_angel = 0
    right_angel = 0
    x_left, x_right = 0, 0
    xplace = 0
    X_place = 0

    leftHandOnScreen = 0
    rightHandOnScreen = 0
    y = 0
    x = 0
    while True:
        left_hand_depth = 0
        right_hand_depth = 0
        y_left_angel = -1000;
        y_right_angel = -1000;
        finger_count_right, finger_count_left = -1, -1
        start_time = dt.datetime.today().timestamp()  # Necessary for FPS calculations
        # Get and align frames
        # Process images
        # Process hands
        # Display FPS
        # Display images
        # Get and align frames
        frames = pipeline.wait_for_frames()
        aligned_frames = align.process(frames)
        aligned_depth_frame = aligned_frames.get_depth_frame()

        # aligned_depth_frame_dec = rs.decimation_filter(2).process(aligned_depth_frame)
        # aligned_depth_frame =aligned_depth_frame_dec

        color_frame = aligned_frames.get_color_frame()
        # color_frame_dec = rs.decimation_filter(2).process(color_frame)
        # color_frame =color_frame_dec
        color_intrin = color_frame.profile.as_video_stream_profile().intrinsics
        if not aligned_depth_frame or not color_frame:
            continue

        # Process images
        depth_image = np.asanyarray(aligned_depth_frame.get_data())
        # depth_image_dec =np.asanyarray(aligned_depth_frame_dec.get_data())

        depth_image_flipped = cv2.flip(depth_image, 1)
        color_image = np.asanyarray(color_frame.get_data())
        depth_image_3d = np.dstack((depth_image, depth_image, depth_image))
        #    background_removed = np.where((depth_image_3d > clipping_distance) | (depth_image_3d <= 0),
        #                                background_removed_color, color_image)

        depth_colormap = cv2.applyColorMap(cv2.convertScaleAbs(depth_image, alpha=0.1), cv2.COLORMAP_JET)
        depth_colormap_flip = cv2.flip(depth_colormap, 1)
        images = cv2.flip(color_image, 1)
        color_image = cv2.flip(color_image, 1)
        color_images_rgb = cv2.cvtColor(color_image, cv2.COLOR_BGR2RGB)

        # Process hands
        results = hands.process(color_images_rgb)
        if results.multi_hand_landmarks:
            number_of_hands = len(results.multi_hand_landmarks)
            i = 0
            min_left_depth = 99999
            min_right_depth = 99999
            min_right = -1
            min_left = -1


            i = 0
            leftHandOnScreen = 0
            rightHandOnScreen = 0
            for handLms in results.multi_hand_landmarks:

                mpDraw.draw_landmarks(images, handLms, mpHands.HAND_CONNECTIONS)
                org2 = (20, org[1] + (20 * (i + 1)))
                hand_side_classification_list = results.multi_handedness[i]
                hand_side = hand_side_classification_list.classification[0].label

                if (number_of_hands == 1):
                    if hand_side == "Left":
                        leftHandOnScreen = 1
                    if hand_side == "Right":
                        rightHandOnScreen = 1
                if number_of_hands == 2:
                    rightHandOnScreen = 1
                    leftHandOnScreen = 1

                # print(rightHandOnScreen)
                # print("\n")
                # print(leftHandOnScreen)

                # middle_finger_knuckle = results.multi_hand_landmarks[i].landmark[9]

                wrist_place = results.multi_hand_landmarks[i].landmark[0]
                x = int(wrist_place.x * len(depth_image_flipped[0]))
                y = int(wrist_place.y * len(depth_image_flipped))
                if x >= len(depth_image_flipped[0]):
                    x = len(depth_image_flipped[0]) - 1
                if y >= len(depth_image_flipped):
                    y = len(depth_image_flipped) - 1

                if (x < 0):
                    x = 0
                if x > 640:
                    x = 640

                # print(f"x={dx},y={dy},z={dz}")
                if hand_side == "Right":
                    finger_count_right = count_fingers(results.multi_hand_landmarks, i, "right")

                if hand_side == "Left":
                    finger_count_left = count_fingers(results.multi_hand_landmarks, i, "left")
                mfk_distance = depth_image_flipped[y, x] * depth_scale  # meters
                mfk_distance_feet = mfk_distance * 3.281  # feet

                # images = cv2.putText(images,f"{hand_side} Hand Distance: {mfk_distance_feet:0.3} feet ({mfk_distance:0.3} m) away", org2, font, fontScale, color, thickness, cv2.LINE_AA)
                # images = cv2.putText(images, f"{hand_side} Hand Distance: x={dx} y={dy} z= {dz} ", org2, font, fontScale, color,thickness, cv2.LINE_AA)
                # images = cv2.putText(images, f" Hand Distance:{hand_diff} ", org2, font, fontScale, color, thickness,
                #                     cv2.LINE_AA)
                if (hand_side == "Right"):
                    right_hand_depth = mfk_distance
                    x_right = x
                if (hand_side == "Left"):
                    left_hand_depth = mfk_distance
                    x_left = x
                if (right_hand_depth > 0 and left_hand_depth > 0):
                    hand_diff = (right_hand_depth - left_hand_depth)
                else:
                    hand_diff = - 1000
                if (x_left > 0 and x_right > 0 and abs(x_right - x_left) < 300):
                    xplace = (x_left + x_right) / 2
                i += 1

                if hand_side == "Left" and y > 0:
                    y_left_angel = y
                elif hand_side == "Right" and y > 0:
                    y_right_angel = y

                # images = cv2.putText(images,
                #                     f"Hands: {number_of_hands}  rightisclosed={is_right_hand_closed}   leftisclosed={is_left_hand_closed}",
                #                     org, font, fontScale, color, thickness, cv2.LINE_AA)
            # else:
            # images = cv2.putText(images, "No Hands", org, font, fontScale, color, thickness, cv2.LINE_AA)
            # 3Display FPS
            time_diff = dt.datetime.today().timestamp() - start_time
            fps = int(1 / time_diff)
            org3 = (20, org[1] + 60)
            # images = cv2.putText(images, f"FPS: {fps}  angel = {angel}", org3, font, fontScale, color, thickness, cv2.LINE_AA)
            name_of_window = 'SN: ' + str(device)

            # Display images

            # cv2.namedWindow(name_of_window, cv2.WINDOW_AUTOSIZE)
            # cv2.imshow(name_of_window, images)
            # cv2.imshow(name_of_window,depth_colormap_flip)
            # key = cv2.waitKey(5)

            '''try to send to server but the server didnt take matrix it only take string need to fix that'''
            #    print(color_images_rgb)      #   s.send(color_images_rgb)

            # Press esc or 'q' to close the image window

        # Display images
        # name_of_window = 'SN: ' + str(device)

        # cv2.namedWindow(name_of_window, cv2.WINDOW_AUTOSIZE)
        # cv2.imshow(name_of_window, images)
        # cv2.imshow(name_of_window,depth_colormap_flip)
        key = cv2.waitKey(5)

        is_left_hand_closed, is_right_hand_closed = 0, 0

        if (finger_count_left < 4):
            is_left_hand_closed = 1

        if (finger_count_right < 4):
            is_right_hand_closed = 1
        if leftHandOnScreen == 0:
            is_left_hand_closed = -1
        if rightHandOnScreen == 0:
            is_right_hand_closed = -1

        #print(str(finger_count_right) + " " + str(finger_count_left))

        # print(f"right hand is closed{is_right_hand_closed}\n")
        # print(f"left hand is closed{is_left_hand_closed}")

        # checking the handdistance

        if hand_diff != -1000:
            bow_power = int(1000 * (hand_diff / 0.6))
            if (bow_power > 1000):
                bow_power = 1000
            elif bow_power < -1000:
                bow_power = -1000
        else:
            bow_power = 2000

        if y_left_angel != -1000:
            left_angel = ((240 - y_left_angel) * 110 / 240)
            left_angel = round(left_angel, 2)
            if (left_angel > 40):
                left_angel = 40
            if (left_angel < -20):
                left_angel = -20
        else:
            left_angel = -1000
        if y_right_angel != -1000:
            right_angel = ((240 - y_right_angel) * 110 / 240)
            right_angel = round(right_angel, 2)
            if (right_angel > 40):
                right_angel = 40
            if (right_angel < -20):
                right_angel = -20
        else:
            right_angel = -1000

        if (xplace > 0 and xplace < 640 and is_right_hand_closed != -1 and is_left_hand_closed != -1):
            X_place = -(320 - xplace) / 300
            X_place = int(X_place * 2000)
            if X_place < -1000:
                X_place = -1000
            if X_place > 1000:
                X_place = 1000
        else:
            X_place = -2000

        left_angel = int(left_angel)
        right_angel = int(right_angel)
        send_data = str(left_angel) + "," + str(bow_power) + "," + str(finger_count_right) + "," + str(
            finger_count_left) + "," + str(int(X_place)) + "," + str(right_angel)
        #print(send_data)
        # print(f"data = {send_data} ")

        # print(len(images))
        # print(len(depth_colormap))
        # need to send images and depth_colormap
        sock.send(send_data.encode("UTF-8"))  # Converting string to Byte, and sending it to C#
        image_mode = sock.recv(1024).decode("UTF-8")  # receiveing data in Byte fron C#, and converting it to String
        if image_mode == "depth" or image_mode == "rgb":
            if image_mode == "rgb":
                shown_image = images
            elif image_mode == "depth":
                shown_image = depth_colormap_flip
            shown_image = cv2.resize(np.asarray(shown_image), (640,480), interpolation=cv2.INTER_AREA)
            _, img_encoded = cv2.imencode('.jpg', shown_image)
            img_data = img_encoded.tobytes()
            size_string = str(len(img_data))
            size_size_string = str(len(size_string))
            sock.send(size_size_string.encode("UTF-8"))
            sock.send(size_string.encode("UTF-8"))
            sock.send(img_data)
            del img_data
            del send_data

    print(f"Exiting loop for SN: {device}")
    print(f"Application Closing.")
    pipeline.stop()
    print(f"Application Closed.")
    sock.close()

    '''note we have depth {depth}
  angel {angel}
  x,y,z{dx,dy,dz}
  hand closed or not {is_hand_closed(bool )}'''


if __name__ == "__main__":
    main()









