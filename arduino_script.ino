String mensagem;

bool isHighCozinha;
bool isHighSala;
bool isHighQuarto;
bool isHighBanheiro;

int pinCozinha = 11;
int pinSala = 10;
int pinQuarto = 9;
int pinBanheiro = 6;

void setup() {
    Serial.begin(9600);
    isHighCozinha = false;
    isHighSala = false;
    isHighQuarto = false;
    isHighBanheiro = false;
    mensagem = "";
    pinMode(pinCozinha, OUTPUT);
    pinMode(pinSala, OUTPUT);
    pinMode(pinQuarto, OUTPUT);
    pinMode(pinBanheiro, OUTPUT);
}

int myParseString(String str) {
    int num = 0;
    for (int i = 0; i < str.length(); i++) {
        num += ((int)(str[i] - 48)) * (int)(pow(10, str.length() - i - 1));
    }

    return num;
}

void loop() {
    bool flag = false;
    String voice = "";
    while(Serial.available()){
        delay(10);
        char c = Serial.read();
        if (c == '#')
        break;

        mensagem += c;
    }
    if (mensagem.length() > 0){
        String intHelper = "";
        for(int i = 0; i<mensagem.length(); i++) {
            if (flag)
                intHelper += mensagem[i];
            else if (!flag && mensagem[i]!='/')
                voice += mensagem[i];

            if (mensagem[i] == '/')
                flag = true;
        }
        double intensidade = (myParseString(intHelper) * 255)/100.0;

        if (voice == "acender cozinha") {
            analogWrite(pinCozinha, intensidade);
            isHighCozinha = true;
        } else if (voice == "apagar cozinha" && isHighCozinha){
            digitalWrite(pinCozinha, LOW);
            isHighCozinha = false;
        } else if (voice == "acender quarto"){
            analogWrite(pinQuarto, intensidade);
            isHighQuarto = true;
        } else if (voice == "apagar quarto" && isHighQuarto) {
            digitalWrite(pinQuarto, LOW);
            isHighQuarto = false;
        } else if (voice == "acender sala") {
            analogWrite(pinSala, intensidade);
            isHighSala = true;
        } else if (voice == "apagar sala" && isHighSala) {
            digitalWrite(pinSala, LOW);
            isHighSala = false;
        } else if (voice == "acender banheiro") {
            analogWrite(pinBanheiro, intensidade);
            isHighBanheiro = true;
        } else if (voice == "apagar banheiro" && isHighBanheiro) {
            digitalWrite(pinBanheiro, LOW);
            isHighBanheiro = false;
        } else if (voice == "acender todos") {
            analogWrite(pinCozinha, intensidade);
            analogWrite(pinSala, intensidade);
            analogWrite(pinBanheiro, intensidade);
            analogWrite(pinQuarto, intensidade);
            isHighCozinha = true;
            isHighSala = true;
            isHighBanheiro = true;
            isHighQuarto = true;
        } else if (voice == "apagar todos") {
            if (isHighCozinha) {
                digitalWrite(pinCozinha, LOW);
                isHighCozinha = false;
            }
            if (isHighQuarto) {
                digitalWrite(pinQuarto, LOW);
                isHighQuarto = false;
            }
            if(isHighSala) {
                digitalWrite(pinSala, LOW);
                isHighSala = false;
            }
            if(isHighBanheiro) {
                digitalWrite(pinBanheiro, LOW);
                isHighBanheiro = false;
            }
        }

        mensagem = "";
    }
}