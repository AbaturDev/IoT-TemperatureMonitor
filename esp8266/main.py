import machine, dht, time, network, json, utime, ntptime
from umqtt.simple import MQTTClient


SSID = "<WIFI_SSID>"
PASSWORD = "<WIFI_PASS>"

PIN = 12

BROKER = "<broker_address>"
CLIENT_ID = "<client_id>"
SUBSCRIPTION = b"<sub_name>"
MQTT_USER = "<user>"
MQTT_PASSWORD = "<password>"

STATUS_DATA_ERROR = "dataError"
STATUS_CRITICAL_ERROR = "criticalError"
STATUS_SUCCESS = "success"

def sub_cb(topic, msg):
  print((topic, msg))

def get_datetime():
    return "{:04d}-{:02d}-{:02d} {:02d}:{:02d}:{:02d}".format(*utime.localtime()[:6])

sensor = dht.DHT22(machine.Pin(PIN))

sta_if = network.WLAN(network.STA_IF)
sta_if.active(True)
sta_if.connect(SSID, PASSWORD)

while not sta_if.isconnected():
    print("Connecting to WiFi...")
    time.sleep(5)

client = MQTTClient(CLIENT_ID, BROKER, user=MQTT_USER, password=PASSWORD)
client.set_callback(sub_cb)
client.connect()
client.subscribe(SUBSCRIPTION)

ntptime.settime()
  
while True:
    try:
        try:
            sensor.measure()
        except OSError as ex:
            print("Failed to read data:", ex)
            error_payload = json.dumps({
                "status": STATUS_DATA_ERROR,
                "timestamp": get_datetime(),
                "message": "Failed to read data from sensor"
            })
            client.publish(SUBSCRIPTION, error_payload)
            print("Successfully sent data")
            
            time.sleep(60)
            continue
        
        temperature = sensor.temperature()
        humidity = sensor.humidity()

        print(f"Temperature: {temperature}, Humidity: {humidity}")

        payload = json.dumps({
            "status": STATUS_SUCCESS,
            "timestamp": get_datetime(),
            "temperature": temperature,
            "humidity": humidity
        })

        client.publish(SUBSCRIPTION, payload)
        print("Successfully sent data")

    except OSError as ex:
        print("Critical error occured. Restarting machine")
        error_payload = json.dumps({
            "status": STATUS_CRITICAL_ERROR,
            "timestamp": get_datetime(),
            "message": "Critical failure, restarting"
        })
        client.publish(SUBSCRIPTION, error_payload)
        print("Successfully sent data")
        time.sleep(5)
        machine.reset()
            
    time.sleep(60)
