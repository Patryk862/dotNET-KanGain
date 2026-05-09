#include <SPI.h>
#include <MFRC522.h>

#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SH110X.h>

// ---------- OLED ----------
#define SCREEN_WIDTH 128
#define SCREEN_HEIGHT 64

Adafruit_SH1106G display(
  SCREEN_WIDTH,
  SCREEN_HEIGHT,
  &Wire,
  -1
);

// ---------- RFID ----------
#define SS_PIN 5
#define RST_PIN 4

MFRC522 rfid(SS_PIN, RST_PIN);

// ---------- LED / BUZZER ----------
#define LED_PIN 16
#define BUZZER_PIN 17

void setup() {

  Serial.begin(115200);

  // OLED I2C
  Wire.begin(6, 7);

  // OLED init
  if(!display.begin(0x3C, true)) {
    Serial.println("OLED ERROR");
    while(1);
  }

  display.clearDisplay();
  display.display();

  // RFID SPI
  SPI.begin(12, 13, 11, 5);

  rfid.PCD_Init();

  pinMode(LED_PIN, OUTPUT);
  pinMode(BUZZER_PIN, OUTPUT);

  ekranStart();
}

void loop() {

  if (!rfid.PICC_IsNewCardPresent()) {
    return;
  }

  if (!rfid.PICC_ReadCardSerial()) {
    return;
  }

  String uidString = "";

  for (byte i = 0; i < rfid.uid.size; i++) {

    if (rfid.uid.uidByte[i] < 0x10) {
      uidString += "0";
    }

    uidString += String(rfid.uid.uidByte[i], HEX);
  }

  uidString.toUpperCase();

  Serial.println(uidString);

  // ekran
  display.clearDisplay();

  display.setTextSize(2);
  display.setTextColor(SH110X_WHITE);

  display.setCursor(0, 20);
  display.println("Zapraszamy!");

  display.display();

  // LED
  digitalWrite(LED_PIN, HIGH);

  // buzzer
  for(int i=0; i<2; i++){
    tone(BUZZER_PIN, 5000);
    delay(220);
    noTone(BUZZER_PIN);
  }

  delay(3000);

  digitalWrite(LED_PIN, LOW);

  ekranStart();

  rfid.PICC_HaltA();
}

void ekranStart() {

  display.clearDisplay();

  display.setTextSize(2);
  display.setTextColor(SH110X_WHITE);

  display.setCursor(10, 15);
  display.println("Przyloz");

  display.setCursor(20, 40);
  display.println("karte");

  display.display();
}