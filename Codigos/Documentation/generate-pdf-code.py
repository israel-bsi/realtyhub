import os
from fpdf import FPDF

# 1. Configurações iniciais
DIRETORIO_RAIZ = "."  # Substitua pelo diretório que deseja varrer
EXTENSOES_CODIGO = {".html", ".css", ".cs", ".json", ".razor", ".js"}  # Adicione ou remova conforme necessário
ARQUIVO_SAIDA = "Codigo-Fonte-Completo.pdf"
PASTAS_EXCLUIR = {"docs", "obj", "bin", ".vs"}  # Pastas a serem ignoradas

# 2. Classe estendida de FPDF para melhor controle de texto
class PDFCode(FPDF):
    def __init__(self):
        super().__init__(orientation="P", unit="mm", format="A4")
        # Defina fonte monoespaçada para código
        self.set_font("Courier", size=10)        
        # Margens: 10mm em cada borda
        self.set_margins(10, 10, 10)
        # Altura máxima de linha (aproximadamente 4.5mm para Courier 10)
        self.line_height = self.font_size_pt * 0.35
        # Nota: A biblioteca FPDF base tem limitações com Unicode usando fontes padrão.
        # Caracteres fora do conjunto 'latin-1'/'cp1252' podem causar erros.
        # Para suporte completo a Unicode, considere usar FPDF2 com fontes TTF (ex: DejaVuSansMono).

    def add_code_file(self, caminho_arquivo: str):
        """
        Insere o arquivo no PDF. Adiciona uma página nova para cada arquivo, 
        com o caminho como título, e depois o conteúdo.
        """
        try:
            with open(caminho_arquivo, "r", encoding="utf-8-sig") as f:
                linhas = f.readlines()
        except UnicodeDecodeError:
            try:
                # Se falhar em UTF-8, tente Latin-1
                with open(caminho_arquivo, "r", encoding="latin-1") as f:
                    linhas = f.readlines()
            except Exception as e:
                print(f"Erro ao ler o arquivo {caminho_arquivo} com UTF-8 e Latin-1: {e}")
                linhas = [f"Erro ao ler o arquivo: {e}\n"]
        except Exception as e:
            print(f"Erro inesperado ao abrir ou ler o arquivo {caminho_arquivo}: {e}")
            linhas = [f"Erro inesperado ao ler o arquivo: {e}\n"]


        # Adiciona página nova para o arquivo
        self.add_page()
        
        # Título: caminho relativo ou absoluto
        # Sanitiza o título para evitar problemas de encoding
        try:
            titulo_sanitized = caminho_arquivo.encode('latin-1', 'replace').decode('latin-1')
        except Exception: # Fallback mais genérico se necessário, embora 'replace' deva prevenir isto.
            titulo_sanitized = caminho_arquivo.encode('ascii', 'replace').decode('ascii')

        self.set_font("Arial", "B", 12)
        self.cell(0, 10, txt=titulo_sanitized, ln=True)
        self.ln(2)  # espaço pequeno após o título

        # Volta à fonte monoespaçada
        self.set_font("Courier", size=10)
        # Margem esquerda: já configurada. Largura de texto disponível:
        largura_disponivel = self.w - 2 * self.l_margin

        # Para cada linha do código, quebrar em múltiplas linhas se for muito longa
        for linha in linhas:
            # Remove caracteres de nova linha e tabulações extras
            linha_corrigida = linha.rstrip("\n").replace("\t", "    ")
            # Quebrar texto em pedaços que caibam na largura
            partes = self.multi_cell_split(linha_corrigida, largura_disponivel)
            for parte in partes:
                # Sanitiza a parte do texto para ser compatível com 'latin-1'
                try:
                    parte_sanitized = parte.encode('latin-1', 'replace').decode('latin-1')
                except Exception: # Fallback mais genérico
                    parte_sanitized = parte.encode('ascii', 'replace').decode('ascii')
                
                self.cell(0, self.line_height, txt=parte_sanitized, ln=True)

    def multi_cell_split(self, texto: str, largura_disponivel: float):
        """
        Divide a string 'texto' em uma lista de substrings que se encaixem na largura_disponivel,
        considerando a fonte monoespaçada Courier. Retorna lista de linhas.
        """
        # Em fonte monoespaçada, cada caractere ocupa a mesma largura.
        # Vamos estimar quantos caracteres cabem em uma linha:
        #   largura_disponivel (mm) / largura_caractere(mm) ≈ número de chars por linha
        # fpdf fornece um método para medir texto em mm:
        largura_texto_total = self.get_string_width(texto)
        if largura_texto_total <= largura_disponivel:
            return [texto]

        # Se for maior, precisamos quebrar.
        # Estimativa simples: calcule quantos caracteres cabem:
        # largura de um caractere:
        largura_char = self.get_string_width("W")  # 'W' é um caractere largo; para Courier, ok
        max_chars = int(largura_disponivel / largura_char)

        # Se max_chars for menor que 1, force como 1 para não entrar em loop
        if max_chars < 1:
            max_chars = 1

        linhas = []
        inicio = 0
        while inicio < len(texto):
            fim = inicio + max_chars
            # Para não quebrar no meio de uma palavra, retroceda até um espaço (se possível)
            if fim < len(texto) and texto[fim] != " ":
                retrocesso = texto.rfind(" ", inicio, fim)
                if retrocesso != -1 and retrocesso > inicio:
                    fim = retrocesso
            parte = texto[inicio:fim]
            linhas.append(parte)
            inicio = fim
            # Pula espaços iniciais na próxima parte
            while inicio < len(texto) and texto[inicio] == " ":
                inicio += 1
        return linhas

# 3. Coleta de arquivos de código
def coletar_arquivos_com_exclusao(diretorio_raiz: str, extensoes: set, pastas_excluir: set) -> list:
    """
    Percorre recursivamente o diretorio_raiz, mas exclui qualquer pasta cujo
    nome (relativo ao diretório atual) esteja em pastas_excluir.
    Retorna uma lista de caminhos de arquivos cujas extensões estejam em 'extensoes'.
    
    - pastas_excluir deve conter nomes de pastas (strings) que serão ignoradas.
      Exemplo: {'venv', '__pycache__', 'tests'}
    """
    arquivos_encontrados = []
    for raiz, subpastas, nomes_arquivos in os.walk(diretorio_raiz):
        # 1) Remover, de subpastas, aqueles nomes que queremos excluir:
        #    ao modificar 'subpastas' diretamente, o os.walk não descerá nessas pastas.
        subpastas[:] = [
            pasta for pasta in subpastas 
            if pasta not in pastas_excluir
        ]
        # 2) Agora 'subpastas' já não contém os diretórios excluídos, então o walk
        #    não entrará neles. Seguimos coletando arquivos normalmente:
        for nome in nomes_arquivos:
            _, ext = os.path.splitext(nome)
            if ext.lower() in extensoes:
                caminhos = os.path.join(raiz, nome)
                arquivos_encontrados.append(caminhos)
    return sorted(arquivos_encontrados)

def main():
    pdf = PDFCode()
    arquivos = coletar_arquivos_com_exclusao(DIRETORIO_RAIZ, EXTENSOES_CODIGO, PASTAS_EXCLUIR)
    if not arquivos:
        print("Nenhum arquivo de código encontrado nos diretórios especificados.")
        return

    print(f"Arquivos encontrados ({len(arquivos)}):")
    for a in arquivos:
        print("  -", a)

    # Adiciona cada arquivo ao PDF
    for idx, caminho in enumerate(arquivos, start=1):
        print(f"[{idx}/{len(arquivos)}] Adicionando: {caminho}")
        pdf.add_code_file(caminho)

    # Salva o PDF
    pdf.output(ARQUIVO_SAIDA)
    print(f"PDF gerado com sucesso: {ARQUIVO_SAIDA}")

if __name__ == "__main__":
    main()
