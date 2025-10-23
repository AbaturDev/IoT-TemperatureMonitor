import machine, dht, time

SSID = ""
PASSWORD = ""

PIN = 6

sensor = dht.DHT22(machine.Pin(PIN))

while True:
    try:
        sensor.measure()
        print("Temperature:", sensor.temperature(), "Humidity:", sensor.humidity())
    except OSError as ex:
        print("Failed to read data:", ex)
    time.sleep(60)


# import network, time
# from umqtt.simple import MQTTClient
# import dht, machine

# # WiFi
# sta_if = network.WLAN(network.STA_IF)
# sta_if.active(True)
# sta_if.connect('Twoje_SSID','Twoje_Haslo')
# while not sta_if.isconnected():
#     time.sleep(1)

# # MQTT
# client = MQTTClient('esp8266_1','adres_brokera')
# client.connect()
# sensor = dht.DHT22(machine.Pin(4))

# while True:
#     sensor.measure()
#     temp = sensor.temperature()
#     hum = sensor.humidity()
#     payload = '{{"temperature": {}, "humidity": {}}}'.format(temp, hum)
#     client.publish(b'sensor/pokoj/1', payload)
#     print('Wysłano:', payload)
#     time.sleep(60)